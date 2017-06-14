using FacebookPollCounter.Excel;
using FacebookPollCounter.Helpers;
using FacebookPollCounter.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FacebookPollCounter.ViewModels
{
    public class MainWindowVM : BindableBase
    {
        #region Constructor
        public MainWindowVM()
        {
            Token = (Application.Current as App).Settings.AccessToken;
            PostUrl = (Application.Current as App).Settings.PostUrl;
            Path = (Application.Current as App).Settings.FilePath;

            SaveCommand = new DelegateCommand(Save);
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

        #endregion

        #region Commands
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand BrowseCommand { get; set; }
        public DelegateCommand HelpCommand { get; set; }
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

                var postId = await FacebookHelper.GetPostIdFromUrl(Token, PostUrl);

                if (postId == null)
                {
                    MessageBox.Show("Could not get the post Id");
                    IsBusy = false;
                    return;
                }

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

                } while (list.Count < totalComments);

                SLExcelData data = GetNewDataObject();
                foreach (var comment in list)
                {
                    data.DataRows.Add(new List<string>() { comment.From.Id, comment.From.Name,
                        comment.CreatedTime.ToString("g"), comment.Message, FacebookHelper.GetVotes(comment.Message) });
                }

                byte[] file = SLExcelWriter.GenerateExcel(data);
                using (var fileStream = File.Create(Path, file.Length))
                {
                    await fileStream.WriteAsync(file, 0, file.Length);
                }
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
        #endregion
    }
}
