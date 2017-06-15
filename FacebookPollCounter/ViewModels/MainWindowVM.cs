using FacebookPollCounter.Excel;
using FacebookPollCounter.Helpers;
using FacebookPollCounter.Mvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FacebookPollCounter.ViewModels
{
    public class MainWindowVM : BindableBase, IDataErrorInfo
    {
        #region Fields
        private const string TOKEN_HELP_URL = "https://rebrand.ly/FbPollToken";
        private CancellationTokenSource _tokenSource;
        #endregion

        #region Constructor
        public MainWindowVM()
        {
            _tokenSource = new CancellationTokenSource();

            Token = (Application.Current as App).Settings.AccessToken;
            PostUrl = (Application.Current as App).Settings.PostUrl;
            Path = (Application.Current as App).Settings.FilePath;

            SaveCommand = new DelegateCommand(Save, o => Error == string.Empty);
            BrowseCommand = new DelegateCommand(o => SetPath((Window)o));
            HelpCommand = new DelegateCommand(o => Process.Start(TOKEN_HELP_URL));
            CancelCommand = new DelegateCommand(o => _tokenSource.Cancel());
        }
        #endregion

        #region Properties
        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private string _token;
        public string Token
        {
            get { return _token; }
            set { SetProperty(ref _token, value); }
        }

        private string _postUrl;
        public string PostUrl
        {
            get { return _postUrl; }
            set { SetProperty(ref _postUrl, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _progress;
        public string Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand BrowseCommand { get; set; }
        public DelegateCommand HelpCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        #endregion

        #region Methods
        private async void Save(Object o)
        {
            IsBusy = true;
            try
            {
                // Save settings
                (Application.Current as App).Settings.AccessToken = Token;
                (Application.Current as App).Settings.PostUrl = PostUrl;
                (Application.Current as App).Settings.FilePath = Path;

                Progress = "Getting Post Id...";

                var postId = await FacebookHelper.GetPostIdFromUrl(Token, PostUrl);

                if (postId == null)
                {
                    MessageBox.Show("Could not get the post/page Id");
                    IsBusy = false;
                    return;
                }

                _tokenSource.Token.ThrowIfCancellationRequested();
                Progress = "Downloading comments...";

                var list = new List<Comment>();
                int totalComments = 1;
                string from = null;
                string after = null;
                do
                {
                    var pagedList = await FacebookHelper.GetComments(Token, postId, from, after).ConfigureAwait(false);
                    if (pagedList.Children.Count == 0)
                        break;

                    totalComments = pagedList.Summary.TotalCount;
                    after = pagedList.Paging.Cursors.After;
                    list.AddRange(pagedList.Children);

                    _tokenSource.Token.ThrowIfCancellationRequested();
                    Progress = $"Downloading comments ({list.Count} of {totalComments})...";

                } while (list.Count < totalComments);

                _tokenSource.Token.ThrowIfCancellationRequested();
                Progress = "Saving Excel file...";

                SLExcelData data = GetNewDataObject();
                foreach (var comment in list)
                {
                    data.DataRows.Add(new List<string>() { comment.From.Id, comment.From.Name,
                        comment.CreatedTime.ToString("g"), comment.Message, FacebookHelper.GetVotes(comment.Message) });
                }

                byte[] file = SLExcelWriter.GenerateExcel(data);
                using (var fileStream = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    await fileStream.WriteAsync(file, 0, file.Length);
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private SLExcelData GetNewDataObject()
        {
            var data = new SLExcelData();
            data.SheetName = "Poll";
            data.Headers = new List<string>() { "Person Id", "Person Name", "Date", "Comment", "Vote" };

            return data;
        }

        private void ValidationErrorStatusChanged()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void SetPath(Window owner)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Excel Workbook|*.xlsx";
            dialog.Title = "Choose a location to save the file in";

            var result = dialog.ShowDialog(owner);

            if (result == true)
            {
                Path = dialog.FileName;
            }
        }
        #endregion

        #region IDataErrorInfo
        private string _oldError = string.Empty;

        public string Error => this[null];

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                var anyProperty = columnName == null;

                if (anyProperty || columnName == nameof(Path))
                    error = ValidatePath(error);
                if (anyProperty || columnName == nameof(PostUrl))
                    error = ValidateUrl(error);
                if (anyProperty || columnName == nameof(Token))
                    error = ValidateToken(error);

                if (_oldError != error)
                {
                    _oldError = error;
                    ValidationErrorStatusChanged();
                }

                return error;
            }
        }

        private string ValidatePath(string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return "Path must not be empty.";
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
                return "The chosen directory does not exist.";

            return defaultValue;
        }

        private string ValidateToken(string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(Token))
                return "Access token must not be empty.";

            return defaultValue;
        }

        private string ValidateUrl(string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(PostUrl))
                return "Post Url must not be empty.";

            bool validUrl = Uri.TryCreate(PostUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttps);
            if (!validUrl) return "Invalid Url";

            if (!PostUrl.Contains("facebook.com"))
                return "Please enter a Facebook post's url";

            return defaultValue;
        }
        #endregion

    }
}
