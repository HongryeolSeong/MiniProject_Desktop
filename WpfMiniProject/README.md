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
###### 
<br/>
<br/>
<br/>

### 5. 네이버 영화 웹사이트 연결
---
![웹사이트 열기](https://github.com/HongryeolSeong/MiniProject_Desktop/blob/main/NMFimg/link.gif)
