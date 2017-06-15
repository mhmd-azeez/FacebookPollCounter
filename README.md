# Facebook Poll Counter
A simple app to count the votes on a facebook post.

**Note:** It can only be used on Page posts.

### Examples of valid URLs:
 - https://www.facebook.com/facebook/videos/vb.20531316728/10155849356871729/?type=3&theater
 - https://www.facebook.com/Engineering/posts/10155327408707200
 - https://www.facebook.com/facebook/photos/a.376995711728.190761.20531316728/10154597567106729/?type=3&theater

## Why do I need to sign into facebook?
This app uses Facebook Graph API. So it needs an access token to be able to use it. The app does not ask for any special permssion, it only gets your public info.

### But I don't want to type in my username and password into your app!
Fair enough. You don't have to. Facebook Poll Counter uses an embedded Internet Explorer to do the login. So you only need to open Internet Explorer, login there (Make sure to check "Remember me") and now you can use the Facebook Poll Counter without needing to "log in" in the app.
 
## How to get the URL of any facebook post
 - Click on the timestamp of the post
 - Copy the address bar URL
