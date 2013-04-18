using System;
using System.Windows.Forms;

namespace Wuqi.Webdiyer
{
    public partial class LayoutForm : Form
    {
        private LayoutType layoutType;
        private PagingButtonLayoutType pagingButtonLayoutType;

        public LayoutType LayoutType
        {
            get { return layoutType; }
            set { layoutType = value; }
        }

        public PagingButtonLayoutType PagingButtonLayoutType
        {
            get { return pagingButtonLayoutType; }
            set { pagingButtonLayoutType = value; }
        }

        public LayoutForm(LayoutType layoutType,PagingButtonLayoutType buttonLayoutType)
        {
            InitializeComponent();
            this.layoutType = layoutType;
            this.pagingButtonLayoutType = buttonLayoutType;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                layoutType = LayoutType.Table;
            else
                layoutType = LayoutType.Div;

            if (radioButton3.Checked)
                pagingButtonLayoutType = PagingButtonLayoutType.UnorderedList;
            else if (radioButton4.Checked)
                pagingButtonLayoutType = PagingButtonLayoutType.Span;
            else
                pagingButtonLayoutType = PagingButtonLayoutType.None;
        }

        private void LayoutForm_Load(object sender, EventArgs e)
        {
            if (layoutType == LayoutType.Table)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            switch(pagingButtonLayoutType)
            {
                case PagingButtonLayoutType.UnorderedList:
                    radioButton3.Checked = true; break;
                case PagingButtonLayoutType.Span:
                    radioButton4.Checked = true; break;
                default:
                    radioButton5.Checked = true;break;
            }
        }
    }
}
