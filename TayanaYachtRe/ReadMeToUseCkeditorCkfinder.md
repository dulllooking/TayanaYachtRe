# Using Ckeditor+Ckfinder with ASP .NET Web Forms C\#

## 安裝方法

1. Visual Studio 專案內用 Nuget 安裝 `CkeditorForASP.NET (3.6.4)` 並安裝附帶更新後重開檔案確認 `CKEditor` 控制項已出現在工具箱 (在.asp 頁切換到設計模式後查看工具箱)

2. CKFinder 官網下載 asp .net 用檔案 (2.6.2)，解壓縮後資料夾放入專案同層，刪除資料夾內無用檔案 (_samples及_source資料夾)  
參考影片 : [CKEditor and CKFinder Integration with asp.net webform](https://www.youtube.com/watch?v=zGUw1BQOafI&t=2s)

3. 專案 "參考" 右鍵加入參考 ( CKEditor.NET.dll 已經用 Nuget 安裝了)，瀏覽選擇 `ckfinder\bin\Release\CKFinder.dll` 加入 dll 檔

4. 開啟 ckfinder 資料夾內的 `config.ascx`，將 `CheckAuthentication()` 功能改為 `return true;`，還有 `SetConfig()` 的 `BaseUrl` 改成上傳檔案的資料夾位置

```csharp
public override bool CheckAuthentication()
{
  return true;
}

public override void SetConfig()
{
  BaseUrl = "/Tayanahtml/upload/";
}
```

## 錯誤排除

1. 如果有使用 Nuget 安裝 Microsoft.AspNet.FriendlyUrls 來隱藏副檔名，會導致 CKFinder 上傳圖片功能錯誤

2. 於上傳圖片頁面用網頁檢查工具比對可以發現導致錯誤原因為 **Status Code: 301**

3. 於全域應用程式類別 Global.asax.cs 修改原本的設定如下

```csharp
protected void Application_Start(object sender, EventArgs e)
{
    // 設定不顯示副檔名 (如果只想隱藏副檔名做到此區塊就好)
    var routes = RouteTable.Routes;
    var settings = new FriendlyUrlSettings();
    settings.AutoRedirectMode = RedirectMode.Permanent;
    //routes.EnableFriendlyUrls(settings);

    //修改避免 CKFinder 上傳功能錯誤
    routes.EnableFriendlyUrls(settings, new Microsoft.AspNet.FriendlyUrls.Resolvers.IFriendlyUrlResolver[] { new MyWebFormsFriendlyUrlResolver() });

    // 執行短網址路由方法
    RegisterRouters(RouteTable.Routes);
}

public class MyWebFormsFriendlyUrlResolver : Microsoft.AspNet.FriendlyUrls.Resolvers.WebFormsFriendlyUrlResolver
{
    public override string ConvertToFriendlyUrl(string path)
    {
        //字串為 ckfinder 固定內容
        if (!string.IsNullOrEmpty(path) && path.ToLower().Contains("/ckfinder/core/connector/aspx/connector.aspx")) {
            return path;
        }
        return base.ConvertToFriendlyUrl(path);
    }
}
```

## 使用方法

- 從工具箱拉入 CKEditor 控制項並檢查上方有無增加引入如下

```html
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
```

- CKEditor 控制項使用時需加入位置屬性 `BasePath="/Scripts/ckeditor/"`

```html
<CKEditor:CKEditorControl ID="CKEditorControl1" runat="server" BasePath="/Scripts/ckeditor/"></CKEditor:CKEditorControl>
```

- 於使用控制項的 .aspx 頁後端 .cs 的 Page_Load 事件內加入以下程式碼

```csharp
FileBrowser fileBrowser = new FileBrowser();
fileBrowser.BasePath = "/ckfinder";
fileBrowser.SetupCKEditor(CKEditorControl1);
```

- 於網頁後置程式碼 .aspx.cs 取得輸入內容編譯方法如下

```csharp
Label1.Text = HttpUtility.HtmlEncode(CKEditorControl1.Text);
```

## 自訂 CKEditor 控制項工具方法

- 可以拿掉用不到的的工具及排列組合

```html
<CKEditor:CKEditorControl ID="CKEditorControl1" runat="server" BasePath="/Scripts/ckeditor/"
 Toolbar="Bold|Italic|Underline|Strike|Subscript|Superscript|-|RemoveFormat
        NumberedList|BulletedList|-|Outdent|Indent|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock|-|BidiLtr|BidiRtl
        /
        Styles|Format|Font|FontSize
        TextColor|BGColor
        Link|Image" >
</CKEditor:CKEditorControl>
```

> 參考文件 : [CKEditor Toolbar](http://docs-old.ckeditor.com/CKEditor_3.x/Developers_Guide/Toolbar)

## 刪除用不到的功能

- **刪除圖片上傳功能用不到的分頁** : 於 ckeditor 資料夾 config.js 客製功能內加入以下程式碼

```js
config.removeDialogTabs = 'image:Upload;image:advanced;image:Link';
```

- **刪除輸入框下方計算字數功能** : 於 ckeditor 資料夾 config.js 客製功能內加入以下程式碼

```js
config.removePlugins = 'elementspath';
```
