using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Reptile.instrument;
using Reptile.Models;
using System.Linq;

namespace Reptile.Dispose
{
    /// <summary>
    /// 内容抓取
    /// </summary>
    public class CommoditySearch
    {
       public List<Category>  _Categories { get; private set; }

        public List<GoodsImgInfo> _ImgInfos { get; set; }


        public CommoditySearch()
        {
            this._Categories = new List<Category>();
            this._ImgInfos = new List<GoodsImgInfo>();
        }

        public HtmlNode HtmlNodeList(string url)
        {
            string html = HttpHelper.DownloadUrl(url);//下载html
            HtmlDocument document = new HtmlDocument(); //初始化容器
            document.LoadHtml(html); //状态Html
            return document.DocumentNode;
        }
        public HtmlNode HtmlNodeList(HtmlNode node,string path)
        {
            return node.SelectSingleNode(path);
        }

        public void HtmlNodeList(string path, HtmlNode Basenodes, Action<HtmlNode> action)
        {
            HtmlNodeCollection nodes = Basenodes.SelectNodes(path);
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    action.Invoke(item);
                }
            }
        }


        public void Start()
        {
            try
            {
                string path = "//*[@class=\"items\"]";
                HtmlNodeList(path, HtmlNodeList("https://www.jd.com/allSort.aspx"), (p) => { this.CategoryMenList(p); });
                CommodityList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 匹配所有类别菜单区域
        /// </summary>
        /// <param name="node"></param>
        private void CategoryMenList(HtmlNode node)
        {
            string path = "./*[@class=\"clearfix\"]";
            HtmlNodeList(path, node, (p) => { this.CategoryArray(p); });
        }

        /// <summary>
        ///匹配所有类别
        /// </summary>
        private void CategoryArray(HtmlNode node)
        {
            string path = "./dd/a";
            HtmlNodeList(path, node, (p) =>
            {
                _Categories.Add(new Category() { _CategoryName = p.InnerText, _CategoryURL = $"https:{p.Attributes["href"].Value}" });
                Console.WriteLine($"类别名称：{p.InnerText}，URL：https:{p.Attributes["href"].Value}");
            });
        }

        /// <summary>
        /// 根据类别请求类别详情页面
        /// </summary>
        private void CommodityList()
        {
            List<Category> Cat = _Categories.Where(p => p._CategoryURL.Contains("?cat=")).ToList();
            foreach (var item in Cat)
            {
                HtmlNode node = HtmlNodeList(item._CategoryURL);
                Console.WriteLine($"类别：{item._CategoryName}\r\n");
                GoodsPaging(node, item._CategoryURL);
              
            }
        }

        /// <summary>
        /// 获取分页
        /// </summary>
        /// <param name="node"></param>
        /// <param name="Url"></param>
        private void GoodsPaging(HtmlNode node,string Url)
        {
            string path = "//*[@class=\"fp-text\"]/i";
            HtmlNode Pagenode = HtmlNodeList(node, path);
            if (Pagenode != null)
            {
                string PageCount = Pagenode.InnerText;
                if (!string.IsNullOrWhiteSpace(PageCount))
                {
                    int Page = Convert.ToInt32(PageCount);
                    for (int i = 1; i <= Page; i++)
                    {
                        string PgeUrl = $"{Url}&page={i}";
                        HtmlNode htmlNome = HtmlNodeList(PgeUrl);
                        GoodsChunk(i == 1 ? node : htmlNome);
                        Console.WriteLine($"Page : {i} \r\n");
                    }
                }
            } 
        }

        /// <summary>
        /// /匹配商品区域
        /// </summary>
        /// <param name="node"></param>
        private void GoodsChunk(HtmlNode node)
        {
            string path = "//*[@id=\"plist\"]";
            HtmlNodeList(path, node, (p) => { Goodslist(p); });
        }

        /// <summary>
        ///匹配商品区域列表
        /// </summary>
        /// <param name="node"></param>
        private void Goodslist(HtmlNode node)
        {
            string path = "./ul/*[@class=\"gl-item\"]";
            HtmlNodeList(path, node, (p) => { GoodList(p); });
        }

        /// <summary>
        ///匹配商品展示列表
        /// </summary>
        /// <param name="node"></param>
        private void GoodList(HtmlNode node)
        {
            string path = "./*[@class=\"gl-i-wrap j-sku-item\"]/*[@class=\"p-name\"]/a/em"; //匹配商品名称
            string Imgpath = "./*[@class=\"gl-i-wrap j-sku-item\"]/*[@class=\"p-img\"]/a/img"; //匹配商品图片
        
            HtmlNodeList(path, node, (p) => { Console.WriteLine($"商品名称：{ p.InnerText.Trim()}");  });
            HtmlNodeList(Imgpath, node, (p) =>
            {
                var img = p.Attributes["src"];
                if (img == null)
                {
                    Console.WriteLine($"商品图片：https:{p.Attributes["data-lazy-img"].Value}");
                    FileImgHelper.Down($"https:{p.Attributes["data-lazy-img"].Value}");
                }
                else
                {
                    Console.WriteLine($"商品图片：https:{p.Attributes["src"].Value}");
                    FileImgHelper.Down($"https:{p.Attributes["src"].Value}");
                }
            });
        }






    }
}
