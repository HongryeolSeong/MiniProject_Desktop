# Naver Movie Finder
[네이버 오픈 API](https://developers.naver.com/main/)와 [구글의 YouTube Data API](console.cloud.google.com)를 이용한 영화 검색 프로그램입니다.

## 진행 순서
### 1. 메인 화면[.xaml.cs 👈 ](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/WpfMiniProject/NaverMovieFinderApp/MainWindow.xaml.cs)
---
###### - Visual Studio를 사용하여 WPF기반으로 구현하였습니다.<br/>
###### - 사용 가능한 기능은 즐겨찾기 추가 / 즐겨찾기 보기 / 즐겨찾기 삭제 / 예고편 보기 / 네이버 영화가 있습니다.
![메인 화면](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/main.png)
<br/>
<br/>
<br/>

### 2. 영화 검색
---
![검색](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/search.gif)
###### 1. 텍스트박스에 검색할 영화를 입력받아 _ProcSearchNaverApi_ 메서드를 실행합니다.
```C#
ProcSearchNaverApi(string movieName)
{
    string clientID = "xxxxxxx";
    string clientSecret = "xxxx";
    string openApiUrl = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={movieName}";

    string resJson = Commons.GetOpenApiResult(openApiUrl, clientID, clientSecret);
    var parsedJson = JObject.Parse(resJson);

    int total = Convert.ToInt32(parsedJson["total"]);
    int display = Convert.ToInt32(parsedJson["display"]);

    StsResult.Content = $"{total} 중 {display} 호출 성공!";

    var items = parsedJson["items"];
    var json_array = (JArray)items;

    List<MovieItem> movieItems = new List<MovieItem>();

    foreach (var item in json_array)
    {
        MovieItem movie = new MovieItem(
            Commons.StripHtmlTag(item["title"].ToString()),
            item["link"].ToString(),
            item["image"].ToString(),
            item["subtitle"].ToString(),
            item["pubDate"].ToString(),
            Commons.StripPipe(item["director"].ToString()),
            Commons.StripPipe(item["actor"].ToString()),
            item["userRating"].ToString()
            );

        movieItems.Add(movie);
    }

    this.DataContext = movieItems;
}
```
###### 2. 네이버 오픈 API에서 부여받은 Client ID, Client Secret, Open API URL을 인수로 _GetOpenApiResult_ 메서드를 사용하여 영화 검색 결과를 문자열로 받습니다.
```C#
GetOpenApiResult(string openApiUrl, string clientID, string clientSecret)
{
    WebRequest request = WebRequest.Create(openApiUrl);
    request.Headers.Add("X-Naver-Client-Id", clientID);
    request.Headers.Add("X-Naver-Client-Secret", clientSecret);

    WebResponse response = request.GetResponse();
    Stream stream = response.GetResponseStream();
    StreamReader reader = new StreamReader(stream);

    result = reader.ReadToEnd();

    reader.Close();
    stream.Close();
    response.Close();
    
    return result;
}
```
###### 3. 그 결과는 _StripHtmlTag_ 와 _StripPipe_ 메서드를 통해 가공되어 데이터 그리드에 출력됩니다.
```C#
// HTML 태그 삭제
public static string StripHtmlTag(string text)
{
    return Regex.Replace(text, @"<(.|\n)*?>", ""); // HTML 태그 삭제하는 정규표현식
}

// | 문자 삭제
public static string StripPipe(string text)
{
    if (string.IsNullOrEmpty(text))
    {
        return "";
    }
    else
    {
        return text.Substring(0, text.LastIndexOf("|")).Replace("|", ", ");

    }
}
```
<br/>
<br/>
<br/>

### 3. 즐겨찾기
---
![즐겨찾기](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/favorites.gif)
###### 1. 원하는 영화를 선택 후 즐겨찾기 추가시 선택된 셀의 데이터를 EntityFramework 라이브러리를 사용하여 DB에 저장합니다.
```C#
List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

foreach (MovieItem item in GrdData.SelectedItems)
{
    NaverFavoriteMovies temp = new NaverFavoriteMovies()
    {
        Title = item.Title,
        Link = item.Link,
        Image = item.Image,
        SubTitle = item.SubTitle,
        PubDate = item.PubDate,
        Actor = item.Actor,
        UserRating = item.UserRating,
        RegDate = DateTime.Now
    };

    list.Add(temp);
}

using (var ctx = new OpenApiLabEntities())
{
    ctx.Set<NaverFavoriteMovies>().AddRange(list);
    ctx.SaveChanges();
}
```
###### 2. 즐겨찾기 목록 조회시 EntityFramework를 통해 DB 조회 후 데이터 그리드에 출력합니다.
```C#
List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

using (var ctx = new OpenApiLabEntities())
{
    list = ctx.NaverFavoriteMovies.ToList();
}
this.DataContext = list;
```
###### 3. 즐겨찾기 삭제시 EntityFramework를 통해 선택된 셀의 데이터로 DB에서 검색 후 삭제를 진행합니다.
```C#
List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

foreach (MovieItem item in GrdData.SelectedItems)
{
    NaverFavoriteMovies temp = new NaverFavoriteMovies()
    {
        Title = item.Title,
        Link = item.Link,
        Image = item.Image,
        SubTitle = item.SubTitle,
        PubDate = item.PubDate,
        Actor = item.Actor,
        UserRating = item.UserRating,
        RegDate = DateTime.Now
    };

    list.Add(temp);
}

using (var ctx = new OpenApiLabEntities())
{
    ctx.Set<NaverFavoriteMovies>().RemoveRange(list);
    ctx.SaveChanges();
}
```
<br/>
<br/>
<br/>

### 4. 예고편 보기[.xaml.cs 👈 ](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/WpfMiniProject/NaverMovieFinderApp/TrailerWindow.xaml.cs)
---
![트레일러](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/trailer.gif)
###### 1. 예고편 버튼 클릭시 새로운 WPF창이 열리고 그와 동시에 _LoadDataCollection_ 이 실행되어 YouTube API를 통해 검색된 데이터는 _youtube_ 객체에 담겨 리스트인 _youtubes_ 에 차곡차곡 쌓이게됩니다.
```C#
private async Task LoadDataCollection()
{
    var youtubeService = new YouTubeService(
        new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCEj2ZDsfqi95enEjUzK0MD-dh1NN1DMo4",
            ApplicationName = this.GetType().ToString()
        }
        ) ;

    var request = youtubeService.Search.List("snippet");
    request.Q = LblMovieName.Content.ToString(); // {movieName} 예고편
    request.MaxResults = 10;                     // 사이즈가 너무 크면 프로그램 멈춤

    var response = await request.ExecuteAsync(); // 검색 시작(Youtube OpenAPI 호출)

    foreach (var item in response.Items)
    {
        if (item.Id.Kind.Equals("youtube#video"))
        {
            YoutubeItem youtube = new YoutubeItem()
            {
                Title = item.Snippet.Title,
                Author = item.Snippet.ChannelTitle,
                URL = $"http://www.youtube.com/watch?v={item.Id.VideoId}"
            };

            // 썸네일 이미지
            youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute));
            
            youtubes.Add(youtube);
        }
    }
}
```
###### 2. 1에서 완성된 리스트 _youtubes_ 는 리스트뷰의 콘텐츠로 출력됩니다.
```C#
private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
{
    youtubes = new List<YoutubeItem>(); // 초기화 필수 : 바인딩시 에러 날 수 있음
    ProcSearchYoutubeApi();
}

private async void ProcSearchYoutubeApi()
{
    await LoadDataCollection();
    LsvYoutubeSearch.ItemsSource = youtubes;
}
```
###### 3. 리스트뷰의 한 셀을 더블클릭할 시 Uri의 객체를 생성하여 웹브라우저 컨트롤에 해당 영화의 예고편 유튜브 링크를 출력합니다.
```C#
var video = LsvYoutubeSearch.SelectedItem as YoutubeItem;
BrwYoutubeWatch.Source = new Uri(video.URL, UriKind.RelativeOrAbsolute);
```
<br/>
<br/>
<br/>

### 5. 네이버 영화 웹사이트 연결
---
###### 네이버 영화 버튼 클릭시 선택된 셀의 영화에 대한 링크를 _Process_ 의 _Start_ 를 이용하여 웹브라우저를 새로 띄웁니다.
![웹사이트 열기](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/link.gif)
<br/>
<br/>
<br/>

