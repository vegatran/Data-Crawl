using DataCrawl.Class;
using Newtonsoft.Json;
using PuppeteerSharp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var browserFetcher = new BrowserFetcher();
        List<NewsInPage> elements = new List<NewsInPage>();
        List<CommentOfNews> lstNewsTopRank = new List<CommentOfNews>();

        await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();

        page.DefaultTimeout = 300000;// tuỳ thuộc vào sự phản hồi của server để set thời gian

        #region Đọc data đã lấy sẵn
        string path = Directory.GetCurrentDirectory();
        if (!File.Exists(path +"/jsonData.txt"))
        {
            File.Create(path + "/jsonData.txt");
        }

        var fs = new FileStream(path + "/jsonData.txt", FileMode.Open, FileAccess.ReadWrite);
        StreamReader sReader = new StreamReader(fs);
        string str = sReader.ReadToEnd();
        sReader.Close();
        fs.Close();

        if (str.Length > 0)
        {
            elements = JsonConvert.DeserializeObject<List<NewsInPage>>(str);
            await page.CloseAsync();
        }
        #endregion
        else {
            await page.GoToAsync("https://vnexpress.net/tin-nong");
            #region Lấy danh sách thẻ ngày trong tuần
            await page.WaitForSelectorAsync("div.tabdate-hotnews > a");
            var menuTab = await CrawlData.GetDate(page, "div.tabdate-hotnews > a");
            #endregion

            #region Lấy danh sách tin theo ngày
            foreach (var e in menuTab)
            {
                Console.WriteLine("\nData Type: {0}", e.DataType);
                if (!e.Class.Contains("active"))
                {
                    await page.ClickAsync("a[data-type='" + e.DataType + "']");
                    Thread.Sleep(10000);// tuỳ thuộc vào sự phản hồi từ server, để thiết lập thời gian.
                    await page.WaitForSelectorAsync("article.item-news");
                    var lstLink = await CrawlData.GetNewsDate(page, "div.list-news-subfolder article.item-news");
                    elements.AddRange(lstLink.Where(x => x.Count_Cmt > 0));
                }
                else
                {
                    await page.WaitForSelectorAsync("article.item-news");
                    var lstLink = await CrawlData.GetNewsDate(page, "div.list-news-subfolder article.item-news");
                    elements.AddRange(lstLink.Where(x => x.Count_Cmt > 0));
                }
            }
            #endregion

            await File.WriteAllTextAsync(path + "/jsonData.txt", JsonConvert.SerializeObject(elements), encoding: System.Text.Encoding.UTF8);
            
        }

        #region Top 10 bài viết trong tuần
        if (!File.Exists(path + "/jsonDataRank.txt"))
        {
            File.Create(path + "/jsonDataRank.txt");
        }
        string strRank = await File.ReadAllTextAsync(path + "/jsonDataRank.txt");
        #endregion

        if (strRank.Length > 0)
        {
            lstNewsTopRank = JsonConvert.DeserializeObject<List<CommentOfNews>>(strRank);
        }
        else
        {
            #region Đọc chi tiết của từng tin để lấy ra danh sách các tin có nhiều người tương tác nhất
            foreach (var x in elements.OrderByDescending(x => x.Count_Cmt).Take(10))
            {
                var pageDetail = await browser.NewPageAsync();
                Console.WriteLine(x.Url);
                pageDetail.DefaultTimeout = 300000;// tuỳ thuộc vào sự phản hồi của server để set thời gian
                try
                {
                    await pageDetail.GoToAsync(x.Url);
                    await pageDetail.WaitForSelectorAsync("div.main_show_comment");
                    var lstComment = await CrawlData.GetDetailNews(pageDetail, "div.comment_item");
                    CommentOfNews objSum = new CommentOfNews
                    {
                        Url = lstComment.FirstOrDefault().Url,
                        Count_Like = lstComment.Sum(x => x.Count_Like)
                    };
                    lstNewsTopRank.Add(objSum);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Link bị lỗi" + x.Url);
                    Console.WriteLine("Chi tiết lỗi: " + e.Message);
                }
                await pageDetail.CloseAsync();
            }
            #endregion
            await File.WriteAllTextAsync(path + "/jsonDataRank.txt", JsonConvert.SerializeObject(lstNewsTopRank), encoding: System.Text.Encoding.UTF8);
        }

        await browser.CloseAsync();
        

        lstNewsTopRank.OrderByDescending(x => x.Count_Like).ToList().ForEach(x =>
        {
            Console.WriteLine("Url: " + x.Url);
            Console.WriteLine("Like: " + x.Count_Like);
        });
        Console.WriteLine("Total news: " + elements.Count);
        Console.ReadLine();
    }
}

public class CrawlData {
    public async static Task<List<Pages>> GetDate(IPage page, string selectorNode)
    {
        var tabDate = await page.EvaluateFunctionAsync<List<Pages>>(@"()=>{
            var linkPage=[];
            document.querySelectorAll('" + selectorNode +"').forEach(" +
                    "e => { " +
                    "let objData = {}; " +
                    "objData.class = $(e).attr('class'); " +
                    "objData.datatype = $(e).attr('data-type');" +
                    "linkPage.push(objData); " +
                    "}); " +
            "return linkPage;}");
        return tabDate;
    }

    public async static Task<List<NewsInPage>> GetNewsDate(IPage page, string selectorNode)
    {
        var lstLink = await page.EvaluateFunctionAsync<List<NewsInPage>>(@"()=>{
                let elements=[];
                document.querySelectorAll('"+ selectorNode + "').forEach(" +
                        "e => { let objData = {}; " +
                        "let title=$(e).find('.title-news').find('a')[0]; " +
                        "objData.Title = title.innerText;  " +
                        "objData.Url = title.href;  " +
                        "objData.Count_Cmt = $(e).find('a.count_cmt').find('span').html(); " +
                        "elements.push(objData); " +
                        "}); " +
                "return elements;}");
        return lstLink;
    }

    public async static Task<List<CommentOfNews>> GetDetailNews(IPage page, string selectorNode)
    {
        var lstComment = await page.EvaluateFunctionAsync<List<CommentOfNews>>(@"()=>{
                var lstComments=[];
                document.querySelectorAll('" + selectorNode + "').forEach(" +
                        "e => {" +
                            "let objData = {};" +
                            "objData.NameComment = $(e).find('.nickname').html();" +
                            "objData.Url = document.URL;" +
                            "objData.Count_Like = $.isNumeric($(e).find('.total_like').html()) ? $(e).find('.total_like').html() : 0;" +
                            "lstComments.push(objData);" +
                        "});" +
                "return lstComments;}"
                );
        return lstComment;
    }
}