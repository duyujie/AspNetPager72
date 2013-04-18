using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wuqi.Webdiyer
{
    public partial class UrlPagingForm : Form
    {
        private bool reverseUrlPageIndex;
        private bool enableUrlRewrite;
        private string urlRewritePattern;
        private bool urlPaging;
        private string urlPageIndexName;

        public bool UrlPaging
        {
            get { return urlPaging; }
            set { urlPaging = value; }
        }

        public bool ReverseUrlPageIndex
        {
            get { return reverseUrlPageIndex; }
            set { reverseUrlPageIndex = value; }
        }

        public bool EnableUrlRewriting
        {
            get { return enableUrlRewrite; }
            set { enableUrlRewrite = value; }
        }

        public string UrlRewritePattern
        {
            get { return urlRewritePattern; }
            set { urlRewritePattern = value; }
        }

        public string UrlPageIndexName
        {
            get { return urlPageIndexName; }
            set { urlPageIndexName = value; }
        }

        public UrlPagingForm(string urlPageIndexName,bool urlPaging,bool reversePageIndex,bool enableRewrite,string rewritePattern)
        {
            InitializeComponent();
            this.urlPageIndexName = urlPageIndexName;
            this.urlPaging = urlPaging;
            this.reverseUrlPageIndex = reversePageIndex;
            this.enableUrlRewrite = enableRewrite;
            this.urlRewritePattern = rewritePattern;
        }

        private void llbl_2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowPopup(checkBox1, "设置Url的重写格式。要重写的Url路径可以是相对于当前的Url路径，如：../pagelist_{0}.aspx ，也可以是绝对路径，如：http://www.webdiyer.com/articles/{0}.aspx ，用“{0}”占位符来表示AspNetPager分页控件的当前页索引值，用“%参数名%”表示Url中相应的参数的值。如果不设置该值，默认值为当前页面路径加下划线后跟页索引， 如当前Url是 http://www.webdiyer.com/articlelist.aspx ，不设置 UrlRewritePattern 时，第一页的默认路径就是 http://www.webdiyer.com/articlelist_1.aspx ，第二页就是 http://www.webdiyer.com/articlelist_2.asp ，依次类推", new Point(Location.X + llbl_2.Bounds.Location.X, Location.Y + llbl_2.Location.Y));
        }

        private void llbl_1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowPopup(checkBox1, "在Url分页时，Url中的当前页索引参数将与分页控件显示的当前页索引值反方向显示，例如：共有25页数据，分页控件的第1页将在Url中显示为25页，分页控件的第25页将在Url中显示为第1页，分页控件中的第2页将在Url中显示为24页，依次类推。", new Point(Location.X + llbl_1.Bounds.Location.X, Location.Y + llbl_1.Location.Y));
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            urlPageIndexName = textBox2.Text.Trim();
            if (checkBox1.Checked || checkBox2.Checked)
                urlPaging = true;
            reverseUrlPageIndex = checkBox1.Checked;
            enableUrlRewrite = checkBox2.Checked;
            urlRewritePattern = textBox1.Text.Trim();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            llbl_2.Enabled=textBox1.Enabled= checkBox2.Checked;
        }

        private void UrlPagingForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = reverseUrlPageIndex;
            checkBox2.Checked = enableUrlRewrite;
            textBox1.Text = urlRewritePattern;
            textBox2.Text = urlPageIndexName;
        }
    }
}
