//AspNetPager分页控件源代码：
//版权所有：陕西省吴起县博杨计算机有限公司 Webdiyer (www.webdiyer.com)；
//此源代码仅供学习参考，不得用作任何商业用途；
//您可以修改并重新编译该控件，但源代码中的版权信息必须原样保留！
//有关控件升级及新控件发布信息，请留意 www.webdiyer.com 。


using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Wuqi.Webdiyer
{
	#region AspNetPager Server Control

	/// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Class[@name="AspNetPager"]/*'/>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [DefaultProperty("PageSize")]
    [DefaultEvent("PageChanged")]
    [ParseChildren(false)]
    [PersistChildren(false)]
    [ANPDescription("desc_AspNetPager")]
    [Designer(typeof(PagerDesigner))]
    [ToolboxData("<{0}:AspNetPager runat=server></{0}:AspNetPager>")]
    [System.Drawing.ToolboxBitmap(typeof(AspNetPager),"AspNetPager.bmp")]
    public class AspNetPager : Panel, INamingContainer, IPostBackEventHandler, IPostBackDataHandler
    {
        #region Private fields
        private string inputPageIndex;
        private string currentUrl;
        private string queryString;
        private AspNetPager cloneFrom;
        private static readonly object EventPageChanging = new object();
        private static readonly object EventPageChanged = new object();
        bool pageChangeEventHandled = false; 
        //AspNetPager Version number
        const string ver = "7.2";

        #endregion

        #region Properties

        #region Navigation Buttons

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowNavigationToolTip"]/*'/>
        [Browsable(true), ANPCategory("cat_Navigation"), DefaultValue(false), ANPDescription("desc_ShowNavigationToolTip"), Themeable(true)]
        public bool ShowNavigationToolTip
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowNavigationToolTip;
                object obj = ViewState["ShowNvToolTip"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowNvToolTip"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NavigationToolTipTextFormatString"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), ANPDefaultValue("def_NavigationToolTipTextFormatString"), ANPDescription("desc_NavigationToolTipTextFormatString")]
        public string NavigationToolTipTextFormatString
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NavigationToolTipTextFormatString;
                object obj = ViewState["NvToolTipFormatString"];
                if (obj == null)
                {
                    if (ShowNavigationToolTip)
                        return SR.GetString("def_NavigationToolTipTextFormatString");
                    return null;
                }
                return (string)obj;
            }
            set
            {
                string tip = value;
                if (tip.Trim().Length < 1 && tip.IndexOf("{0}") < 0)
                    tip = "{0}";
                ViewState["NvToolTipFormatString"] = tip;
            }
        }

        /// <include file='AspnetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NumericButtonTextFormatString"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(""), ANPCategory("cat_Navigation"), ANPDescription("desc_NBTFormatString")]
        public string NumericButtonTextFormatString
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NumericButtonTextFormatString;
                object obj = ViewState["NumericButtonTextFormatString"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["NumericButtonTextFormatString"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CurrentPageButtonTextFormatString"]/*'/>
        [Browsable(true),Themeable(true),DefaultValue(""),ANPCategory("cat_Navigation"),ANPDescription("desc_CPBTextFormatString")]
        public string CurrentPageButtonTextFormatString
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CurrentPageButtonTextFormatString;
                object obj = ViewState["CurrentPageButtonTextFormatString"];
                return (obj == null) ? NumericButtonTextFormatString : (string)obj;
            }
            set
            {
                ViewState["CurrentPageButtonTextFormatString"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagingButtonType"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(PagingButtonType.Text), ANPCategory("cat_Navigation"), ANPDescription("desc_PagingButtonType")]
        public PagingButtonType PagingButtonType
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PagingButtonType;
                object obj = ViewState["PagingButtonType"];
                return (obj == null) ? PagingButtonType.Text : (PagingButtonType)obj;
            }
            set
            {
                ViewState["PagingButtonType"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NumericButtonType"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(PagingButtonType.Text), ANPCategory("cat_Navigation"), ANPDescription("desc_NumericButtonType")]
        public PagingButtonType NumericButtonType
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NumericButtonType;
                object obj = ViewState["NumericButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["NumericButtonType"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagingButtonLayoutType"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(PagingButtonLayoutType.None), ANPCategory("cat_Navigation"), ANPDescription("desc_PagingButtonLayoutType")]
	    public PagingButtonLayoutType PagingButtonLayoutType
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PagingButtonLayoutType;
                object obj = ViewState["PagingButtonLayoutType"];
                return (obj == null) ? PagingButtonLayoutType.None : (PagingButtonLayoutType) obj;
            }
            set { ViewState["PagingButtonLayoutType"] = value; }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CurrentPageButtonPosition"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(PagingButtonPosition.Fixed), ANPCategory("cat_Navigation"), ANPDescription("desc_CurrentPageButtonPosition")]
	    public PagingButtonPosition CurrentPageButtonPosition
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CurrentPageButtonPosition;
                object obj = ViewState["CurrentPageButtonPosition"];
                return (obj == null) ? PagingButtonPosition.Fixed : (PagingButtonPosition) obj;
            }
            set { ViewState["CurrentPageButtonPosition"] = value; }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NavigationButtonType"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), DefaultValue(PagingButtonType.Text), ANPDescription("desc_NavigationButtonType")]
        public PagingButtonType NavigationButtonType
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NavigationButtonType;
                object obj = ViewState["NavigationButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["NavigationButtonType"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="UrlPagingTarget"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(""), TypeConverter(typeof(TargetConverter)), ANPCategory("cat_Navigation"), ANPDescription("desc_UrlPagingTarget")]
        public string UrlPagingTarget
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.UrlPagingTarget;
                return (string)ViewState["UrlPagingTarget"];
            }
            set
            {
                ViewState["UrlPagingTarget"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="MoreButtonType"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), DefaultValue(PagingButtonType.Text), ANPDescription("desc_MoreButtonType")]
        public PagingButtonType MoreButtonType
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.MoreButtonType;
                object obj = ViewState["MoreButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["MoreButtonType"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagingButtonSpacing"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), DefaultValue(typeof(Unit), "5px"), ANPDescription("desc_PagingButtonSpacing")]
        public Unit PagingButtonSpacing
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PagingButtonSpacing;
                object obj = ViewState["PagingButtonSpacing"];
                return (obj == null) ? Unit.Pixel(5) : (Unit.Parse(obj.ToString()));
            }
            set
            {
                ViewState["PagingButtonSpacing"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowFirstLast"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_ShowFirstLast"), ANPCategory("cat_Navigation"), DefaultValue(true)]
        public bool ShowFirstLast
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowFirstLast;
                object obj = ViewState["ShowFirstLast"];
                return (obj == null) ? true : (bool)obj;
            }
            set { ViewState["ShowFirstLast"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowPrevNext"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_ShowPrevNext"), ANPCategory("cat_Navigation"), DefaultValue(true)]
        public bool ShowPrevNext
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowPrevNext;
                object obj = ViewState["ShowPrevNext"];
                return (obj == null) ? true : (bool)obj;
            }
            set { ViewState["ShowPrevNext"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowPageIndex"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_ShowPageIndex"), ANPCategory("cat_Navigation"), DefaultValue(true)]
        public bool ShowPageIndex
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowPageIndex;
                object obj = ViewState["ShowPageIndex"];
                return (obj == null) ? true : (bool)obj;
            }
            set { ViewState["ShowPageIndex"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="FirstPageText"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_FirstPageText"), ANPCategory("cat_Navigation"), DefaultValue("&lt;&lt;")]
        public string FirstPageText
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.FirstPageText;
                object obj = ViewState["FirstPageText"];
                return (obj == null) ? "&lt;&lt;" : (string)obj;
            }
            set { ViewState["FirstPageText"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PrevPageText"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_PrevPageText"), ANPCategory("cat_Navigation"), DefaultValue("&lt;")]
        public string PrevPageText
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PrevPageText;
                object obj = ViewState["PrevPageText"];
                return (obj == null) ? "&lt;" : (string)obj;
            }
            set { ViewState["PrevPageText"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NextPageText"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_NextPageText"), ANPCategory("cat_Navigation"), DefaultValue("&gt;")]
        public string NextPageText
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NextPageText;
                object obj = ViewState["NextPageText"];
                return (obj == null) ? "&gt;" : (string)obj;
            }
            set { ViewState["NextPageText"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="LastPageText"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_LastPageText"), ANPCategory("cat_Navigation"), DefaultValue("&gt;&gt;")]
        public string LastPageText
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.LastPageText;
                object obj = ViewState["LastPageText"];
                return (obj == null) ? "&gt;&gt;" : (string)obj;
            }
            set { ViewState["LastPageText"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NumericButtonCount"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_NumericButtonCount"), ANPCategory("cat_Navigation"), DefaultValue(10)]
        public int NumericButtonCount
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NumericButtonCount;
                object obj = ViewState["NumericButtonCount"];
                return (obj == null) ? 10 : (int)obj;
            }
            set { ViewState["NumericButtonCount"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowDisabledButtons"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), ANPDescription("desc_ShowDisabledButtons"), DefaultValue(true)]
        public bool ShowDisabledButtons
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowDisabledButtons;
                object obj = ViewState["ShowDisabledButtons"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["ShowDisabledButtons"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowMoreButtons"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_Navigation"), ANPDescription("desc_ShowMoreButtons"), DefaultValue(true)]
	    public bool ShowMoreButtons
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowMoreButtons;
                object obj = ViewState["ShowMoreButtons"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["ShowMoreButtons"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CenterCurrentPageButton"]/*'/>
        [Obsolete("use CurrentPageButtonPosition instead"),Browsable(false),Themeable(false),ANPCategory("Behavior"),ANPDescription("desc_CenterCurrentPageButton"),DefaultValue(false)]
        public bool CenterCurrentPageButton
        {
            get
            {
                return CurrentPageButtonPosition == PagingButtonPosition.Center;
            }
            set
            {
                if (value)
                    CurrentPageButtonPosition = PagingButtonPosition.Center;
            }
        }

        #endregion

        #region Image Navigation Buttons

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ImagePath"]/*'/>
        [Browsable(true), Category("Appearance"), ANPDescription("desc_ImagePath"), DefaultValue(null)]
        public string ImagePath
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ImagePath;
                string imgPath = (string)ViewState["ImagePath"];
                if (imgPath != null)
                    imgPath = ResolveUrl(imgPath);
                return imgPath;
            }
            set
            {
                string imgPath = value.Trim().Replace("\\", "/");
                ViewState["ImagePath"] = (imgPath.EndsWith("/")) ? imgPath : imgPath + "/";
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ButtonImageExtension"]/*'/>
        [Browsable(true), Themeable(true), Category("Appearance"), DefaultValue(".gif"), ANPDescription("desc_ButtonImageExtension")]
        public string ButtonImageExtension
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ButtonImageExtension;
                object obj = ViewState["ButtonImageExtension"];
                return (obj == null) ? ".gif" : (string)obj;
            }
            set
            {
                string ext = value.Trim();
                ViewState["ButtonImageExtension"] = (ext.StartsWith(".")) ? ext : ("." + ext);
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ButtonImageNameExtension"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(null), Category("Appearance"), ANPDescription("desc_ButtonImageNameExtension")]
        public string ButtonImageNameExtension
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ButtonImageNameExtension;
                object obj = ViewState["ButtonImageNameExtension"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["ButtonImageNameExtension"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CpiButtonImageNameExtension"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(null), Category("Appearance"), ANPDescription("desc_CpiButtonImageNameExtension")]
        public string CpiButtonImageNameExtension
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CpiButtonImageNameExtension;
                object obj = ViewState["CpiButtonImageNameExtension"];
                return (obj == null) ? ButtonImageNameExtension : (string)obj;
            }
            set
            {
                ViewState["CpiButtonImageNameExtension"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="DisabledButtonImageNameExtension"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(null), Category("Appearance"), ANPDescription("desc_DisabledButtonImageNameExtension")]
        public string DisabledButtonImageNameExtension
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.DisabledButtonImageNameExtension;
                object obj = ViewState["DisabledButtonImageNameExtension"];
                return (obj == null) ? ButtonImageNameExtension : (string)obj;
            }
            set
            {
                ViewState["DisabledButtonImageNameExtension"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ButtonImageAlign"]/*'/>
        [Browsable(true), ANPDescription("desc_ButtonImageAlign"), DefaultValue(ImageAlign.NotSet), Category("Appearance")]
        public ImageAlign ButtonImageAlign
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ButtonImageAlign;
                object obj = ViewState["ButtonImageAlign"];
                return (obj == null) ? ImageAlign.NotSet : (ImageAlign)obj;
            }
            set
            {
                if (value != ImageAlign.Right && value != ImageAlign.Left)
                    ViewState["ButtonImageAlign"] = value;
            }
        }


        #endregion

        #region Paging

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="UrlPaging"]/*'/>
        [Browsable(true), ANPCategory("cat_Paging"), DefaultValue(false), ANPDescription("desc_UrlPaging")]
        public bool UrlPaging
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.UrlPaging;
                object obj = ViewState["UrlPaging"];
                return (null == obj) ? false : (bool)obj;
            }
            set
            {
                ViewState["UrlPaging"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="UrlPageIndexName"]/*'/>
        [Browsable(true), DefaultValue("page"), ANPCategory("cat_Paging"), ANPDescription("desc_UrlPageIndexName")]
        public string UrlPageIndexName
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.UrlPageIndexName;
                object obj = ViewState["UrlPageIndexName"];
                return (null==obj)?"page":(string)obj;
            }
            set { ViewState["UrlPageIndexName"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="UrlPageSizeName"]/*'/>
        [Browsable(true),DefaultValue(""),ANPCategory("cat_Paging"),ANPDescription("desc_UrlPageSizeName")]
	    public string UrlPageSizeName
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.UrlPageSizeName;
                return (string)ViewState["UrlPageSizeName"];
            }
            set { ViewState["UrlPageSizeName"] = value; }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ReverseUrlPageIndex"]/*'/>
        [Browsable(true),DefaultValue(false),ANPCategory("cat_Paging"),ANPDescription("desc_ReverseUrlPageIndex")]
        public bool ReverseUrlPageIndex
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ReverseUrlPageIndex;
                object obj = ViewState["ReverseUrlPageIndex"];
                return (null == obj) ? false : (bool)obj;
            }
            set { ViewState["ReverseUrlPageIndex"] = value; }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CurrentPageIndex"]/*'/>
        [Browsable(true), ANPDescription("desc_CurrentPageIndex"), ANPCategory("cat_Paging"), DefaultValue(1)]
        public int CurrentPageIndex
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CurrentPageIndex;
                object cpage = ViewState["CurrentPageIndex"];
                int pindex = (cpage==null)?1 : (int)cpage;
                if (pindex > PageCount && PageCount > 0)
                    return PageCount;
                if (pindex < 1)
                    return 1;
                return pindex;
            }
            set
            {
                int cpage = value;
                if (cpage < 1)
                    cpage = 1;
                //else if (cpage > PageCount)
                //    cpage = PageCount;
                ViewState["CurrentPageIndex"] = cpage;
                if(!pageChangeEventHandled)
                {
                    OnPageChanging(new PageChangingEventArgs(cpage));
                }
            }
        }

        //added by DUYJ at 2009.02.18
        [Browsable(true), ANPDescription("desc_StoredPageIndex"), ANPCategory("cat_Paging"), DefaultValue(1)]
        public int StoredPageIndex
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.StoredPageIndex;
                else
                {
                    object obj = ViewState["StoredPageIndex"];
                    return (null == obj) ? 1 : (int)obj;
                }
            }
            set
            {
                if( value<1)
                    ViewState["StoredPageIndex"] = 1;
                else
                    ViewState["StoredPageIndex"] = value;
               
            }
        }
        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="RecordCount"]/*'/>
        [Browsable(false), ANPDescription("desc_RecordCount"), Category("Data"), DefaultValue(0)]
        public int RecordCount
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.RecordCount;
                object obj = ViewState["Recordcount"];
                return (obj == null) ? 0 : (int)obj;
            }
            set { ViewState["Recordcount"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagesRemain"]/*'/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PagesRemain
        {
            get
            {
                return PageCount - CurrentPageIndex;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageSize"]/*'/>
        [Browsable(true), ANPDescription("desc_PageSize"), ANPCategory("cat_Paging"), DefaultValue(10)]
        public int PageSize
        {
            get
            {
                if (!string.IsNullOrEmpty(UrlPageSizeName) && !DesignMode)
                {
                    int pageSize;
                    if (int.TryParse(Page.Request.QueryString[UrlPageSizeName], out pageSize))
                    {
                        if (pageSize > 0)
                            return pageSize;
                    }
                }
                if (null != cloneFrom)
                    return cloneFrom.PageSize;
                object obj = ViewState["PageSize"];
                return (obj == null) ? 10 : (int)obj;
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="RecordsRemain"]/*'/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecordsRemain
        {
            get
            {
                if (CurrentPageIndex < PageCount)
                    return RecordCount - (CurrentPageIndex * PageSize);
                return 0;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="StartRecordIndex"]/*'/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StartRecordIndex
        {
            get
            {
                //if (FillLastPage && RecordCount > PageSize && CurrentPageIndex == PageCount)
                //    return RecordCount - PageSize;
                return (CurrentPageIndex - 1) * PageSize + 1;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="EndRecordIndex"]/*'/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int EndRecordIndex
        {
            get
            {
                return RecordCount - RecordsRemain;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageCount"]/*'/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageCount
        {
            get
            {
                if (RecordCount == 0)
                    return 1;
                return (int)Math.Ceiling((double)RecordCount / (double)PageSize);
            }
        }


        #endregion

        #region Page index box


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowPageIndexBox"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(ShowPageIndexBox.Auto), ANPDescription("desc_ShowPageIndexBox")]
        public ShowPageIndexBox ShowPageIndexBox
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowPageIndexBox;
                object obj = ViewState["ShowPageIndexBox"];
                return (obj == null) ? ShowPageIndexBox.Auto : (ShowPageIndexBox)obj;
            }
            set { ViewState["ShowPageIndexBox"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageIndexBoxType"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(PageIndexBoxType.TextBox), ANPDescription("desc_PageIndexBoxType")]
        public PageIndexBoxType PageIndexBoxType
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PageIndexBoxType;
                object obj = ViewState["PageIndexBoxType"];
                return (obj == null) ? PageIndexBoxType.TextBox : (PageIndexBoxType)obj;
            }
            set
            {
                ViewState["PageIndexBoxType"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageIndexBoxClass"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(null), ANPDescription("desc_PageIndexBoxClasss")]
        public string PageIndexBoxClass
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PageIndexBoxClass;
                object obj = ViewState["PageIndexBoxClass"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                if (value.Trim().Length > 0)
                    ViewState["PageIndexBoxClass"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageIndexBoxStyle"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(null), ANPDescription("desc_PageIndexBoxStyle")]
        public string PageIndexBoxStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PageIndexBoxStyle;
                object obj = ViewState["PageIndexBoxStyle"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                if (value.Trim().Length > 0)
                    ViewState["PageIndexBoxStyle"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="TextBeforePageIndexBox"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(null), ANPDescription("desc_TextBeforePageIndexBox")]
        public string TextBeforePageIndexBox
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.TextBeforePageIndexBox;
                object obj = ViewState["TextBeforePageIndexBox"];
                return (null == obj) ? null : (string)obj;
            }
            set
            {
                ViewState["TextBeforePageIndexBox"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="TextAfterPageIndexBox"]/*'/>
        [Browsable(true), Themeable(true), DefaultValue(null), ANPCategory("cat_PageIndexBox"), ANPDescription("desc_TextAfterPageIndexBox")]
        public string TextAfterPageIndexBox
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.TextAfterPageIndexBox;
                object obj = ViewState["TextAfterPageIndexBox"];
                return (null == obj) ? null : (string)obj;
            }
            set
            {
                ViewState["TextAfterPageIndexBox"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="SubmitButtonText"]/*'/>
        [Browsable(true), Themeable(true), ANPCategory("cat_PageIndexBox"), DefaultValue("go"), ANPDescription("desc_SubmitButtonText")]
        public string SubmitButtonText
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.SubmitButtonText;
                object obj = ViewState["SubmitButtonText"];
                return (null == obj) ? "go" : (string)obj;
            }
            set
            {
                if (null == value)
                    value = "go";
                ViewState["SubmitButtonText"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="SubmitButtonClass"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(null), ANPDescription("desc_SubmitButtonClass")]
        public string SubmitButtonClass
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.SubmitButtonClass;
                return (string)ViewState["SubmitButtonClass"];
            }
            set
            {
                ViewState["SubmitButtonClass"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="SubmitButtonStyle"]/*'/>
        [Browsable(true), ANPCategory("cat_PageIndexBox"), DefaultValue(null), ANPDescription("desc_SubmitButtonStyle")]
        public string SubmitButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.SubmitButtonStyle;
                return (string)ViewState["SubmitButtonStyle"];
            }
            set
            {
                ViewState["SubmitButtonStyle"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="SubmitButtonImageUrl"]/*'/>
        [Browsable(true),DefaultValue(""),Category("Appearance"),ANPDescription("desc_SubmitButtonImageUrl")]
        public string SubmitButtonImageUrl
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.SubmitButtonImageUrl;
                return (string) ViewState["SubmitButtonImageUrl"];
            }
            set { ViewState["SubmitButtonImageUrl"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowBoxThreshold"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_ShowBoxThreshold"), ANPCategory("cat_PageIndexBox"), DefaultValue(30)]
        public int ShowBoxThreshold
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowBoxThreshold;
                object obj = ViewState["ShowBoxThreshold"];
                return (null == obj) ? 30 : (int)obj;
            }
            set { ViewState["ShowBoxThreshold"] = value; }
        }


        #endregion

        #region CustomInfoSection

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="ShowCustomInfoSection"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_ShowCustomInfoSection"), DefaultValue(ShowCustomInfoSection.Never), Category("Appearance")]
        public ShowCustomInfoSection ShowCustomInfoSection
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.ShowCustomInfoSection;
                object obj = ViewState["ShowCustomInfoSection"];
                return (null == obj) ? ShowCustomInfoSection.Never : (ShowCustomInfoSection)obj;
            }
            set { ViewState["ShowCustomInfoSection"] = value; }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CustomInfoTextAlign"]/*'/>
        [Browsable(true), Category("Appearance"), DefaultValue(HorizontalAlign.NotSet), ANPDescription("desc_CustomInfoTextAlign")]
        public HorizontalAlign CustomInfoTextAlign
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CustomInfoTextAlign;
                object obj = ViewState["CustomInfoTextAlign"];
                return (null == obj) ? HorizontalAlign.NotSet : (HorizontalAlign)obj;
            }
            set
            {
                ViewState["CustomInfoTextAlign"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CustomInfoSectionWidth"]/*'/>
        [Browsable(true), Category("Appearance"), DefaultValue(typeof(Unit), "40%"), ANPDescription("desc_CustomInfoSectionWidth")]
        public Unit CustomInfoSectionWidth
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CustomInfoSectionWidth;
                object obj = ViewState["CustomInfoSectionWidth"];
                return (null == obj) ? Unit.Percentage(40) : (Unit)obj;
            }
            set
            {
                ViewState["CustomInfoSectionWidth"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CustomInfoClass"]/*'/>
        [Browsable(true), Category("Appearance"), DefaultValue(null), ANPDescription("desc_CustomInfoClass")]
        public string CustomInfoClass
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CustomInfoClass;
                object obj = ViewState["CustomInfoClass"];
                return (null == obj) ? CssClass : (string)obj;
            }
            set
            {
                ViewState["CustomInfoClass"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CustomInfoStyle"]/*'/>
        [Browsable(true), Category("Appearance"), DefaultValue(null), ANPDescription("desc_CustomInfoStyle")]
        public string CustomInfoStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CustomInfoStyle;
                object obj = ViewState["CustomInfoStyle"];
                return (null == obj) ? Style.Value : (string)obj;
            }
            set
            {
                ViewState["CustomInfoStyle"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CustomInfoHTML"]/*'/>
        [Browsable(true), Themeable(true), Category("Appearance"), DefaultValue("Page %CurrentPageIndex% of %PageCount%"), ANPDescription("desc_CustomInfoHTML")]
        public string CustomInfoHTML
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CustomInfoHTML;
                object obj = ViewState["CustomInfoText"];
                return (null == obj) ? "Page %CurrentPageIndex% of %PageCount%" : (string)obj;
            }
            set
            {
                ViewState["CustomInfoText"] = value;
            }
        }

        #endregion

        #region Css class and styles

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CurrentPageButtonStyle"]/*'/>
        [Browsable(true),Category("Appearance"),Themeable(true),ANPDescription("desc_CurrentPageButtonStyle"),DefaultValue(null)]
        public string CurrentPageButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CurrentPageButtonStyle;
                object obj = ViewState["CPBStyle"];
                return (null==obj)?null:(string)obj;
            }
            set
            {
                ViewState["CPBStyle"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CurrentPageButtonClass"]/*'/>
        [Browsable(true), Category("Appearance"),Themeable(true), ANPDescription("desc_CurrentPageButtonClass"), DefaultValue(null)]
        public string CurrentPageButtonClass
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.CurrentPageButtonClass;
                object obj = ViewState["CPBClass"];
                return (null==obj)?null:(string)obj;
            }
            set
            {
                ViewState["CPBClass"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagingButtonClass"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_PagingButtonClass"), DefaultValue(null)]
	    public string PagingButtonClass
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PagingButtonClass;
                object obj = ViewState["PagingButtonClass"];
                return (null == obj) ? null : (string)obj;
            }
            set
            {
                ViewState["PagingButtonClass"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PagingButtonStyle"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_PagingButtonStyle"), DefaultValue(null)]
	    public string PagingButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.PagingButtonStyle;
                object obj = ViewState["PagingButtonStyle"];
                return (null == obj) ? null : (string)obj;
            }
            set
            {
                ViewState["PagingButtonStyle"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="FirstLastButtonClass"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_FirstLastButtonClass"), DefaultValue(null)]
	    public string FirstLastButtonClass
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.FirstLastButtonClass;
                object obj = ViewState["FirstLastButtonClass"];
                return (null == obj) ? PagingButtonClass : (string)obj;
            }
            set
            {
                ViewState["FirstLastButtonClass"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="FirstLastButtonStyle"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_FirstLastButtonStyle"), DefaultValue(null)]
	    public string FirstLastButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.FirstLastButtonStyle;
                object obj = ViewState["FirstLastButtonStyle"];
                return (null == obj) ? PagingButtonStyle : (string)obj;
            }
            set
            {
                ViewState["FirstLastButtonStyle"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NextPrevButtonClass"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_NextPrevButtonClass"), DefaultValue(null)]
	    public string NextPrevButtonClass
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NextPrevButtonClass;
                object obj = ViewState["NextPrevButtonClass"];
                return (null == obj) ? PagingButtonClass : (string)obj;
            }
            set
            {
                ViewState["NextPrevButtonClass"] = value;
            }
	    }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NextPrevButtonStyle"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_NextPrevButtonStyle"), DefaultValue(null)]
	    public string NextPrevButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.NextPrevButtonStyle;
                object obj = ViewState["NextPrevButtonStyle"];
                return (null == obj) ? PagingButtonStyle : (string)obj;
            }
            set
            {
                ViewState["NextPrevButtonStyle"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="MoreButtonClass"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_MoreButtonClass"), DefaultValue(null)]
        public string MoreButtonClass
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.MoreButtonClass;
                object obj = ViewState["MoreButtonClass"];
                return (null == obj) ? PagingButtonClass : (string)obj;
            }
            set
            {
                ViewState["MoreButtonClass"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="MoreButtonStyle"]/*'/>
        [Browsable(true), Category("Appearance"), Themeable(true), ANPDescription("desc_MoreButtonStyle"), DefaultValue(null)]
        public string MoreButtonStyle
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.MoreButtonStyle;
                object obj = ViewState["MoreButtonStyle"];
                return (null == obj) ? PagingButtonStyle : (string)obj;
            }
            set
            {
                ViewState["MoreButtonStyle"] = value;
            }
        }

        #endregion

        #region Others

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="CloneFrom"]/*'/>
        [Browsable(true), Themeable(false), TypeConverter(typeof(AspNetPagerIDConverter)), Category("Behavior"), DefaultValue(false), ANPDescription("desc_CloneFrom")]
        public string CloneFrom
        {
            get
            {
                return (string)ViewState["CloneFrom"];
            }
            set
            {
                if (null != value && String.Empty == value.Trim())
                    throw new ArgumentNullException("CloneFrom", "The Value of property CloneFrom can not be empty string!");
                if (ID.Equals(value, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("The property value of CloneFrom can not be set to control itself!", "CloneFrom");
                ViewState["CloneFrom"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="EnableTheming"]/*'/>
        public override bool EnableTheming
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.EnableTheming;
                return base.EnableTheming;
            }
            set
            {
                base.EnableTheming = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="SkinID"]/*'/>
        public override string SkinID
        {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.SkinID;
                return base.SkinID;
            }
            set
            {
                base.SkinID = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="EnableUrlRewriting"]/*'/>
        [Browsable(true), Themeable(true), Category("Behavior"), DefaultValue(false), ANPDescription("desc_EnableUrlWriting")]
        public bool EnableUrlRewriting
        {
            get
            {
                object obj = ViewState["UrlRewriting"];
                if (null == obj)
                {
                    if (null != cloneFrom)
                        return cloneFrom.EnableUrlRewriting;
                    return false;
                }
                return (bool)obj;
            }
            set
            {
                ViewState["UrlRewriting"] = value;
                if (value)
                    UrlPaging = true;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="UrlRewritePattern"]/*'/>
        [Browsable(true), Themeable(true), Category("Behavior"), DefaultValue(null), ANPDescription("desc_UrlRewritePattern")]
        public string UrlRewritePattern
        {
            get
            {
                object obj = ViewState["URPattern"];
                if (null == obj)
                {
                    if (null != cloneFrom)
                        return cloneFrom.UrlRewritePattern;
                    if (EnableUrlRewriting)
                    {
                        if (!DesignMode)
                        {
                            string filePath = Page.Request.FilePath;
                            return Path.GetFileNameWithoutExtension(filePath) + "_{0}" + Path.GetExtension(filePath);
                        }
                    }
                    return null;
                }
                return (string)obj;
            }
            set
            {
                ViewState["URPattern"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="AlwaysShow"]/*'/>
        [Browsable(true), Themeable(true), Category("Behavior"), DefaultValue(false), ANPDescription("desc_AlwaysShow")]
        public bool AlwaysShow
        {
            get
            {
                object obj = ViewState["AlwaysShow"];
                if (null == obj)
                {
                    if (null != cloneFrom)
                        return cloneFrom.AlwaysShow;
                    return false;
                }
                return (bool)obj;
            }
            set
            {
                ViewState["AlwaysShow"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="Wrap"]/*'/>
        public override bool Wrap
        {
            get
            {
                return base.Wrap;
            }
            set
            {
                base.Wrap = false;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="PageIndexOutOfRangeErrorMessage"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_PIOutOfRangeMsg"), ANPDefaultValue("def_PIOutOfRangerMsg"), Category("Data")]
        public string PageIndexOutOfRangeErrorMessage
        {
            get
            {
                object obj = ViewState["PIOutOfRangeErrorMsg"];
                if (null == obj)
                {
                    if (null != cloneFrom)
                        return cloneFrom.PageIndexOutOfRangeErrorMessage;
                    return SR.GetString("def_PIOutOfRangerMsg");
                }
                return (string)obj;
            }
            set
            {
                ViewState["PIOutOfRangeErrorMsg"] = value;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="InvalidPageIndexErrorMessage"]/*'/>
        [Browsable(true), Themeable(true), ANPDescription("desc_InvalidPIErrorMsg"), ANPDefaultValue("def_InvalidPIErrorMsg"), Category("Data")]
        public string InvalidPageIndexErrorMessage
        {
            get
            {
                object obj = ViewState["InvalidPIErrorMsg"];
                if (null == obj)
                {
                    if (null != cloneFrom)
                        return cloneFrom.InvalidPageIndexErrorMessage;
                    return SR.GetString("def_InvalidPIErrorMsg");
                }
                return (string)obj;
            }
            set
            {
                ViewState["InvalidPIErrorMsg"] = value;
            }
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="LayoutType"]/*'/>
        [Browsable(true),DefaultValue(LayoutType.Div),Themeable(true),ANPDescription("desc_LayoutType"),Category("Appearance")]
	    public LayoutType LayoutType
	    {
            get
            {
                if (null != cloneFrom)
                    return cloneFrom.LayoutType;
                object obj = ViewState["LayoutType"];
                return (null == obj) ? LayoutType.Div : (LayoutType) obj;
            }
            set { ViewState["LayoutType"] = value; }
	    }

        #endregion

        #endregion

        #region Control Rendering Logic

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="OnInit"]/*'/>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (null != CloneFrom && string.Empty != CloneFrom.Trim())
            {
                AspNetPager ctrl = Parent.FindControl(CloneFrom) as AspNetPager;
                if (null == ctrl)
                {
                    string errStr = SR.GetString("clonefromexeption") ??
                                    "The control \" %controlID% \" does not exist or is not of type Wuqi.Webdiyer.AspNetPager!";
                    throw new ArgumentException(errStr.Replace("%controlID%", CloneFrom), "CloneFrom");
                }
                if (null != ctrl.cloneFrom && this == ctrl.cloneFrom)
                {
                    string errStr = SR.GetString("recusiveclonefrom") ??
                                    "Invalid value for the CloneFrom property, AspNetPager controls can not to be cloned recursively!";
                    throw new ArgumentException(errStr, "CloneFrom");
                }
                cloneFrom = ctrl;
                CssClass = cloneFrom.CssClass;
                Width = cloneFrom.Width;
                Style.Value = cloneFrom.Style.Value;
                Height = cloneFrom.Height;
                HorizontalAlign = cloneFrom.HorizontalAlign;
                BackColor = cloneFrom.BackColor;
                BackImageUrl = cloneFrom.BackImageUrl;
                BorderColor = cloneFrom.BorderColor;
                BorderStyle = cloneFrom.BorderStyle;
                BorderWidth = cloneFrom.BorderWidth;
                Font.CopyFrom(cloneFrom.Font);
                ForeColor = cloneFrom.ForeColor;
                EnableViewState = cloneFrom.EnableViewState;
                Enabled = cloneFrom.Enabled;
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="OnLoad"]/*'/>
        protected override void OnLoad(EventArgs e)
        {
            if (UrlPaging)
            {
                /***** DUYJ编辑于2009.02.12******/
                //currentUrl = Page.Request.Path;
                currentUrl = Page.Request.CurrentExecutionFilePath;
                queryString = Page.Request.ServerVariables["Query_String"];
                if (!Page.IsPostBack && cloneFrom == null)
                {
                    int index;


                    int.TryParse(Page.Request.QueryString[UrlPageIndexName], out index);
                    if (index <= 0)
                    {
                        if (this.StoredPageIndex > 1)
                            index = StoredPageIndex;
                        else
                            index = 1;
                    }
                    else if (ReverseUrlPageIndex)
                        index = PageCount - index + 1;
                    
                    PageChangingEventArgs args = new PageChangingEventArgs(index);
                    OnPageChanging(args);
                }
            }
            else
            {
                inputPageIndex = Page.Request.Form[UniqueID + "_input"];
            }
            base.OnLoad(e);
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="OnPreRender"]/*'/>
        protected override void OnPreRender(EventArgs e)
        {
            if (PageCount > 1)
            {
                if ((ShowPageIndexBox == ShowPageIndexBox.Always) || (ShowPageIndexBox == ShowPageIndexBox.Auto && PageCount >= ShowBoxThreshold))
                {
                    StringBuilder sb = new StringBuilder("<script language=\"Javascript\"><!--\n");
                    if (UrlPaging)
                    {
                        sb.Append("function ANP_goToPage(boxEl){if(boxEl!=null){var pi;if(boxEl.tagName==\"SELECT\")");
                        sb.Append("{pi=boxEl.options[boxEl.selectedIndex].value;}else{pi=boxEl.value;}");
                        if(string.IsNullOrEmpty(UrlPagingTarget))
                            sb.Append("location.href=\"").Append(GetHrefString(-1)).Append("\"");
                        else
                            sb.Append("window.open(\"").Append(GetHrefString(-1)).Append("\",\"").Append(UrlPagingTarget).Append("\")");
                        sb.Append(";}}\n");
                    }
                    if (PageIndexBoxType == PageIndexBoxType.TextBox)
                    {
                        string ciscript = SR.GetString("checkinputscript");
                        if (ciscript != null)
                        {
                            ciscript = ciscript.Replace("%PageIndexOutOfRangeErrorMessage%", PageIndexOutOfRangeErrorMessage);
                            ciscript = ciscript.Replace("%InvalidPageIndexErrorMessage%", InvalidPageIndexErrorMessage);
                        }
                        sb.Append(ciscript).Append("\n");
                        string keyScript = SR.GetString("keydownscript");
                        sb.Append(keyScript);
                    }
                    sb.Append("\n--></script>");
                    Type ctype = GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    if (!cs.IsClientScriptBlockRegistered(ctype, "anp_script"))
                        cs.RegisterClientScriptBlock(ctype, "anp_script", sb.ToString());
                }
            }
            base.OnPreRender(e);
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="AddAttributesToRender"]/*'/>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (Page != null) 
            {
                if (!UrlPaging || (UrlPaging && ShowPageIndexBox != ShowPageIndexBox.Never))//UrlPaging mode also need to register client script, so werver form in required
                    Page.VerifyRenderingInServerForm(this);
            }
            base.AddAttributesToRender(writer);
        }

	    /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="RenderBeginTag"]/*'/>
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            bool showPager = (PageCount > 1 || (PageCount <= 1 && AlwaysShow));
            writer.WriteLine();
            writer.Write("<!-- ");
            writer.Write("AspNetPager V" + ver + " for VS2005 & VS2008  Copyright:2003-2008 Webdiyer (www.webdiyer.com)");
            writer.WriteLine(" -->");
            if (!showPager)
            {
                writer.Write("<!--");
                writer.Write(SR.GetString("autohideinfo"));
                writer.Write("-->");
            }
            else
                base.RenderBeginTag(writer);
        }

	    /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="RenderEndTag"]/*'/>
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            if (PageCount > 1 || (PageCount <= 1 && AlwaysShow))
                base.RenderEndTag(writer);
            writer.WriteLine();
            writer.Write("<!-- ");
            writer.Write("AspNetPager V" + ver + " for VS2005 & VS2008 End");
            writer.WriteLine(" -->");
            writer.WriteLine();
        }


        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="RenderContents"]/*'/>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (PageCount <= 1 && !AlwaysShow)
                return;
            writer.Indent = 0;
            if (ShowCustomInfoSection != ShowCustomInfoSection.Never)
            {
                if (LayoutType == LayoutType.Table)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, Style.Value);
                    if (Height != Unit.Empty)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
                    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table); //<table>
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr); //<tr>
                }
                if (ShowCustomInfoSection == ShowCustomInfoSection.Left)
                {
                    RenderCustomInfoSection(writer);
                    RenderNavigationSection(writer);
                }
                else
                {
                    RenderNavigationSection(writer);
                    RenderCustomInfoSection(writer);
                }
                if (LayoutType == LayoutType.Table)
                {
                    writer.RenderEndTag(); //</tr>
                    writer.RenderEndTag(); //</table>
                }
            }
            else
                RenderPagingElements(writer);
        }

	    static void addMoreListItem(HtmlTextWriter writer,int pageIndex)
        {
            writer.Write("<option value=\"");
            writer.Write(pageIndex);
            writer.Write("\">......</option>");
        }

        void listPageIndices(HtmlTextWriter writer, int startIndex, int endIndex)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                writer.Write("<option value=\"");
                writer.Write(i);
                writer.Write("\"");
                if (i == CurrentPageIndex)
                    writer.Write(" selected=\"true\"");
                writer.Write(">");
                writer.Write(i);
                writer.Write("</option>");
            }
        }

        #endregion

        #region Private Helper Functions


        private void RenderCustomInfoSection(HtmlTextWriter writer)
        {
            if (Height != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            string customUnit = CustomInfoSectionWidth.ToString();
            if (CustomInfoClass != null && CustomInfoClass.Trim().Length > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CustomInfoClass);
            if (CustomInfoStyle != null && CustomInfoStyle.Trim().Length > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, CustomInfoStyle);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, customUnit);
            if (CustomInfoTextAlign != HorizontalAlign.NotSet)
                writer.AddAttribute(HtmlTextWriterAttribute.Align, CustomInfoTextAlign.ToString().ToLower());
            if (LayoutType == LayoutType.Div)
            {
                writer.AddStyleAttribute("float", "left");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "bottom");
                writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "true");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
            }
            writer.Write(GetCustomInfoHTML(CustomInfoHTML));
            writer.RenderEndTag();
        }

        private void RenderNavigationSection(HtmlTextWriter writer)
        {
            if (CustomInfoSectionWidth.Type == UnitType.Percentage)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width,
                                         (Unit.Percentage(100 - CustomInfoSectionWidth.Value)).ToString());
            }
            if (HorizontalAlign != HorizontalAlign.NotSet)
                writer.AddAttribute(HtmlTextWriterAttribute.Align, HorizontalAlign.ToString().ToLower());
            if (!string.IsNullOrEmpty(CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            if (LayoutType == LayoutType.Div)
            {
                writer.AddStyleAttribute("float", "left");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //<div>
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "bottom");
                writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "true");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //<td>
            }
            RenderPagingElements(writer);
            writer.RenderEndTag(); //</div> or </td>
        }

        private void RenderPagingElements(HtmlTextWriter writer)
        {
            int startIndex = ((CurrentPageIndex - 1) / NumericButtonCount) * NumericButtonCount; //this is an important trick, it's not the same as CurrentPageIndex-1
            if (PageCount > NumericButtonCount&&CurrentPageButtonPosition!=PagingButtonPosition.Fixed)
            {
                switch(CurrentPageButtonPosition)
                {
                    case PagingButtonPosition.End:
                        if (CurrentPageIndex > NumericButtonCount)
                            startIndex = CurrentPageIndex - NumericButtonCount;
                        break;
                    case PagingButtonPosition.Center:
                        int startOffset = CurrentPageIndex - (int) (Math.Ceiling((double) NumericButtonCount/2));
                        if (startOffset > 0)
                        {
                            startIndex = startOffset;
                            if (startIndex > (PageCount - NumericButtonCount))
                                startIndex = PageCount - NumericButtonCount;
                        }
                        break;
                    case PagingButtonPosition.Beginning:
                        startIndex = CurrentPageIndex - 1;
                        if (startIndex + NumericButtonCount > PageCount)
                            startIndex = PageCount - NumericButtonCount;
                        break;
                }
            }

            int endIndex = ((startIndex + NumericButtonCount) > PageCount) ? PageCount : (startIndex + NumericButtonCount);

            if(PagingButtonLayoutType==PagingButtonLayoutType.UnorderedList)
                writer.RenderBeginTag(HtmlTextWriterTag.Ul); //<ul>

            CreateNavigationButton(writer, PagerItemType.First);
            CreateNavigationButton(writer, PagerItemType.Prev);
            if (ShowPageIndex)
            {
                if (startIndex > 0)
                    CreateMoreButton(writer, startIndex);
                for (int i = startIndex + 1; i <= endIndex; i++)
                {
                    CreateNumericButton(writer, i);
                }
                if (PageCount > NumericButtonCount && endIndex < PageCount)
                    CreateMoreButton(writer, endIndex + 1);
            }
            CreateNavigationButton(writer, PagerItemType.Next);
            CreateNavigationButton(writer, PagerItemType.Last);

            if (PagingButtonLayoutType == PagingButtonLayoutType.UnorderedList)
                writer.RenderEndTag(); //</ul>


            if ((ShowPageIndexBox == ShowPageIndexBox.Always) || (ShowPageIndexBox == ShowPageIndexBox.Auto && PageCount >= ShowBoxThreshold))
            {
                string boxClientId = UniqueID + "_input";
                writer.Write("&nbsp;&nbsp;");
                if (!string.IsNullOrEmpty(TextBeforePageIndexBox))
                    writer.Write(TextBeforePageIndexBox);
                if (PageIndexBoxType == PageIndexBoxType.TextBox) //TextBox
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "30px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, CurrentPageIndex.ToString());
                    if (!string.IsNullOrEmpty(PageIndexBoxStyle))
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, PageIndexBoxStyle);
                    if (!string.IsNullOrEmpty(PageIndexBoxClass))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, PageIndexBoxClass);
                    if (!Enabled || (PageCount <= 1 && AlwaysShow))
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, boxClientId);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, boxClientId);
                    string chkInputScript = "ANP_checkInput(\'" + boxClientId + "\'," + PageCount + ")";
                    string keydownScript = "ANP_keydown(event,\'" + UniqueID + "_btn\');";
                    string clickScript = "if(" + chkInputScript + "){ANP_goToPage(document.getElementById(\'" + boxClientId + "\'));};return false;";

                    writer.AddAttribute("onkeydown", keydownScript);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                    //Text after page index box
                    if (!string.IsNullOrEmpty(TextAfterPageIndexBox))
                        writer.Write(TextAfterPageIndexBox);

                    //button
                    if (!string.IsNullOrEmpty(SubmitButtonImageUrl))
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, SubmitButtonImageUrl);
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, UrlPaging ? "button" : "submit");
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, SubmitButtonText);
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, UniqueID + "_btn");
                    if (!string.IsNullOrEmpty(SubmitButtonClass))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, SubmitButtonClass);
                    if (!string.IsNullOrEmpty(SubmitButtonStyle))
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, SubmitButtonStyle);
                    if (!Enabled || (PageCount <= 1 && AlwaysShow))
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, (UrlPaging) ? clickScript : "if(" + chkInputScript+"){"+Page.ClientScript.GetPostBackEventReference(this,null)+"} else{return false}");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
                else //Dropdownlist
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, boxClientId);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, boxClientId);
                    writer.AddAttribute(HtmlTextWriterAttribute.Onchange, UrlPaging ? "ANP_goToPage(this)" : Page.ClientScript.GetPostBackEventReference(this, null));
                    writer.RenderBeginTag(HtmlTextWriterTag.Select);
                    if (PageCount > 80) //list only part of page indices
                    {
                        if (CurrentPageIndex <= 15)
                        {
                            listPageIndices(writer, 1, 15);
                            addMoreListItem(writer, 16);
                            listPageIndices(writer, PageCount - 4, PageCount);
                        }
                        else if (CurrentPageIndex >= PageCount - 14)
                        {
                            listPageIndices(writer, 1, 5);
                            addMoreListItem(writer, PageCount - 15);
                            listPageIndices(writer, PageCount - 14, PageCount);
                        }
                        else
                        {
                            listPageIndices(writer, 1, 5);
                            addMoreListItem(writer, CurrentPageIndex - 6);
                            listPageIndices(writer, CurrentPageIndex - 5, CurrentPageIndex + 5);
                            addMoreListItem(writer, CurrentPageIndex + 6);
                            listPageIndices(writer, PageCount - 4, PageCount);
                        }
                    }
                    else //list all page indices
                        listPageIndices(writer, 1, PageCount);
                    writer.RenderEndTag();
                    if (!string.IsNullOrEmpty(TextAfterPageIndexBox))
                        writer.Write(TextAfterPageIndexBox);
                }
            }
        }

	    /// <summary>
        /// Get the navigation url for the paging button.
        /// </summary>
        /// <param name="pageIndex">the page index correspond to the button.</param>
        /// <returns>href string for the paging navigation button.</returns>
        private string GetHrefString(int pageIndex)
	    {
	        if (UrlPaging)
	        {
	            int urlPageIndex = pageIndex;
	            string jsValue = "pi";
	            if (ReverseUrlPageIndex)
	            {
	                jsValue = "(" + PageCount + "-pi+1)";
	                urlPageIndex = pageIndex == -1 ? -1 : PageCount - pageIndex + 1;
	            }
	            if (EnableUrlRewriting)
	            {
	                Regex reg = new Regex("(?<p>%(?<m>[^%]+)%)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	                MatchCollection mts = reg.Matches(UrlRewritePattern);
	                string prmValue;
	                NameValueCollection urlParams = ConvertQueryStringToCollection(queryString);
	                string url = UrlRewritePattern;
	                foreach (Match m in mts)
	                {
	                    prmValue = urlParams[m.Groups["m"].Value];
	                    //if (prmValue != null)
	                    url = url.Replace(m.Groups["p"].Value, prmValue);
	                }
	                return
	                    ResolveUrl(string.Format(url,
	                                             (urlPageIndex == -1) ? "\"+" + jsValue + "+\"" : urlPageIndex.ToString()));
	            }
	            return BuildUrlString(UrlPageIndexName,
	                                  (urlPageIndex == -1) ? "\"+" + jsValue + "+\"" : urlPageIndex.ToString());
	        }
	        return Page.ClientScript.GetPostBackClientHyperlink(this, pageIndex.ToString());
	    }

	    /// <summary>
        /// Replace the property placeholders in the CustomInfoHTML with the property values repectively
        /// </summary>
        /// <param name="origText">original CustomInfoHTML</param>
        /// <returns></returns>
        private string GetCustomInfoHTML(string origText)
        {
            if (!string.IsNullOrEmpty(origText) && origText.IndexOf('%') >= 0)
            {
                string[] props = new string[] { "recordcount", "pagecount", "currentpageindex", "startrecordindex", "endrecordindex", "pagesize", "pagesremain", "recordsremain" };
                StringBuilder sb = new StringBuilder(origText);
                Regex reg = new Regex("(?<ph>%(?<pname>\\w{8,})%)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection mts = reg.Matches(origText);
                foreach (Match m in mts)
                {
                    string p = m.Groups["pname"].Value.ToLower();
                    if (Array.IndexOf(props, p) >= 0)
                    {
                        string repValue = null;
                        switch (p)
                        {
                            case "recordcount":
                                repValue = RecordCount.ToString(); break;
                            case "pagecount":
                                repValue = PageCount.ToString(); break;
                            case "currentpageindex":
                                repValue = CurrentPageIndex.ToString(); break;
                            case "startrecordindex":
                                repValue = StartRecordIndex.ToString(); break;
                            case "endrecordindex":
                                repValue = EndRecordIndex.ToString(); break;
                            case "pagesize":
                                repValue = PageSize.ToString(); break;
                            case "pagesremain":
                                repValue = PagesRemain.ToString(); break;
                            case "recordsremain":
                                repValue = RecordsRemain.ToString(); break;
                        }
                        if (repValue != null)
                            sb.Replace(m.Groups["ph"].Value, repValue);
                    }
                }
                return sb.ToString();
            }
            return origText;
        }

        /// <summary>
        /// Convert raw query string to NameValueCollection
        /// </summary>
        /// <param name="s">raw query string</param>
        private static NameValueCollection ConvertQueryStringToCollection(string s)
        {
            NameValueCollection prms = new NameValueCollection();
            int num = (s != null) ? s.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = s[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }
                string skey = null;
                string svalue;
                if (num4 >= 0)
                {
                    skey = s.Substring(startIndex, num4 - startIndex);
                    svalue = s.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    svalue = s.Substring(startIndex, i - startIndex);
                }
                prms.Add(skey,svalue);
                if ((i == (num - 1)) && (s[i] == '&'))
                {
                    prms.Add(null, string.Empty);
                }
            }
            return prms;
        }

        /// <summary>
        /// add paging parameter and value to the current url or change parameter value if it already exists when using url paging mode
        /// </summary>
        /// <param name="sk">name of the url parameter to be added</param>
        /// <param name="sv">value of the url paramter to be added</param>
        /// <returns>href string for the navigattion buttn</returns>
        private string BuildUrlString(string sk,string sv)
        {
            StringBuilder ubuilder=new StringBuilder(80);
            bool keyFound = false;
            int num = (queryString != null) ? queryString.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = queryString[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }
                string skey = null;
                string svalue;
                if (num4 >= 0)
                {
                    skey = queryString.Substring(startIndex, num4 - startIndex);
                    svalue = queryString.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    svalue = queryString.Substring(startIndex, i - startIndex);
                }
                ubuilder.Append(skey).Append("=");
                if (skey == sk)
                {
                    keyFound = true;
                    ubuilder.Append(sv);
                }
                else
                    ubuilder.Append(svalue);
                ubuilder.Append("&");
            }
            if (!keyFound)
                ubuilder.Append(sk).Append("=").Append(sv);
            ubuilder.Insert(0, "?").Insert(0, Path.GetFileName(currentUrl));
            return ubuilder.ToString().Trim('&');
        }

        /// <summary>
        /// Create first, prev, next or last button.
        /// </summary>
        /// <param name="writer">A <see cref="System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        /// <param name="itemType">pager item type</param>
        private void CreateNavigationButton(HtmlTextWriter writer, PagerItemType itemType)
        {
            if (!ShowFirstLast && (itemType ==PagerItemType.First || itemType ==PagerItemType.Last))
                return;
            if (!ShowPrevNext && (itemType ==PagerItemType.Prev || itemType ==PagerItemType.Next))
                return;
            if(itemType==PagerItemType.First||itemType==PagerItemType.Last) //first page or last page button
                AddClassAndStyle(FirstLastButtonClass, FirstLastButtonStyle, writer);
            else
                AddClassAndStyle(NextPrevButtonClass, NextPrevButtonStyle, writer);//next page or prevous page button
            AddPagingButtonLayoutTag(writer); //<li> or <span>

            string linktext;
            bool disabled;
            int pageIndex;
            bool imgButton = (PagingButtonType == PagingButtonType.Image &&
                              NavigationButtonType == PagingButtonType.Image);
            if (itemType == PagerItemType.Prev || itemType ==PagerItemType.First)
            {
                disabled = (CurrentPageIndex <= 1) | !Enabled;
                if (!ShowDisabledButtons && disabled)
                    return;
                pageIndex = (itemType ==PagerItemType.First) ? 1 : (CurrentPageIndex - 1);
                writeSpacingStyle(writer);
                if(PagingButtonLayoutType==PagingButtonLayoutType.None) //add css class and style attribute to pager item directly
                {
                    if (itemType == PagerItemType.First || itemType == PagerItemType.Last) //first page or last page button
                        AddClassAndStyle(FirstLastButtonClass, FirstLastButtonStyle, writer);
                    else
                        AddClassAndStyle(NextPrevButtonClass, NextPrevButtonStyle, writer);//next page or prevous page button
                }
                if (imgButton)
                {
                    if (!disabled)
                    {
                        
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                        AddToolTip(writer, pageIndex);
                        AddHyperlinkTarget(writer);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.AddAttribute(HtmlTextWriterAttribute.Src,
                                            String.Concat(ImagePath, itemType.ToString(), ButtonImageNameExtension,
                                                          ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        if (ButtonImageAlign != ImageAlign.NotSet)
                            writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Src,
                                            String.Concat(ImagePath, itemType.ToString(), DisabledButtonImageNameExtension,
                                                          ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        if (ButtonImageAlign != ImageAlign.NotSet)
                            writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                    }
                }
                else
                {
                    linktext = (itemType ==PagerItemType.Prev) ? PrevPageText : FirstPageText;
                    if (disabled)
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    else
                    {
                        AddToolTip(writer, pageIndex);
                        AddHyperlinkTarget(writer);
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(linktext);
                    writer.RenderEndTag();
                }
            }
            else
            {
                disabled = (CurrentPageIndex >= PageCount) | !Enabled;
                if (!ShowDisabledButtons && disabled)
                    return;
                pageIndex = (itemType ==PagerItemType.Last) ? PageCount : (CurrentPageIndex + 1);
                writeSpacingStyle(writer);
                if (PagingButtonLayoutType == PagingButtonLayoutType.None) //add css class and style attribute to pager item directly
                {
                    if (itemType == PagerItemType.First || itemType == PagerItemType.Last) //first page or last page button
                        AddClassAndStyle(FirstLastButtonClass, FirstLastButtonStyle, writer);
                    else
                        AddClassAndStyle(NextPrevButtonClass, NextPrevButtonStyle, writer);//next page or prevous page button
                }
                if (imgButton)
                {
                    if (!disabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                        AddToolTip(writer, pageIndex);
                        AddHyperlinkTarget(writer);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.AddAttribute(HtmlTextWriterAttribute.Src,
                                            String.Concat(ImagePath, itemType.ToString(), ButtonImageNameExtension,
                                                          ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        if (ButtonImageAlign != ImageAlign.NotSet)
                            writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Src,
                                            String.Concat(ImagePath, itemType.ToString(), DisabledButtonImageNameExtension,
                                                          ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        if (ButtonImageAlign != ImageAlign.NotSet)
                            writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                    }
                }
                else
                {
                    linktext = (itemType ==PagerItemType.Next) ? NextPageText : LastPageText;
                    if (disabled)
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    else
                    {
                        AddToolTip(writer, pageIndex);
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                        AddHyperlinkTarget(writer);
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(linktext);
                    writer.RenderEndTag();
                }
            }
            if (PagingButtonLayoutType != PagingButtonLayoutType.None)
                writer.RenderEndTag(); //</li> or </span>
        }

        /// <summary>
        /// Add tool tip text to navigation button.
        /// </summary>
        private void AddToolTip(HtmlTextWriter writer, int pageIndex)
        {
            if (ShowNavigationToolTip)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Title, String.Format(NavigationToolTipTextFormatString, pageIndex));
            }
        }
        /// <summary>
        /// add paging button layout tag
        /// </summary>
        private void AddPagingButtonLayoutTag(HtmlTextWriter writer)
        {
            if (PagingButtonLayoutType == PagingButtonLayoutType.UnorderedList)
                writer.RenderBeginTag(HtmlTextWriterTag.Li); //<li>
            else if (PagingButtonLayoutType == PagingButtonLayoutType.Span)
                writer.RenderBeginTag(HtmlTextWriterTag.Span); //<span>
        }

        /// <summary>
        /// Create numeric paging button.
        /// </summary>
        /// <param name="writer">A <see cref="System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        /// <param name="index">the page index correspond to the paging button</param>
        private void CreateNumericButton(HtmlTextWriter writer, int index)
        {
            bool isCurrent = (index == CurrentPageIndex);

            if ((!isCurrent && PagingButtonLayoutType != PagingButtonLayoutType.None) || (isCurrent && PagingButtonLayoutType == PagingButtonLayoutType.UnorderedList)) //current page button already wrapped in span
            {
                if (!isCurrent) //exclude current page index button
                    AddClassAndStyle(PagingButtonClass, PagingButtonStyle,writer);
                AddPagingButtonLayoutTag(writer); //<li>
            }

            if (PagingButtonType == PagingButtonType.Image && NumericButtonType == PagingButtonType.Image)
            {
                writeSpacingStyle(writer);
                if (!isCurrent)
                {
                    if (Enabled)
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(index));
                    if (PagingButtonLayoutType == PagingButtonLayoutType.None) //add css class and style attribute to hyper link directory
                        AddClassAndStyle(PagingButtonClass, PagingButtonStyle, writer);
                    AddToolTip(writer, index);
                    AddHyperlinkTarget(writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    CreateNumericImages(writer, index, false);
                    writer.RenderEndTag();
                }
                else
                {
                    AddClassAndStyle(CurrentPageButtonClass, CurrentPageButtonStyle, writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    CreateNumericImages(writer, index, true);
                    writer.RenderEndTag();
                }
            }
            else
            {
                writeSpacingStyle(writer);
                if (isCurrent)
                {
                    if (string.IsNullOrEmpty(CurrentPageButtonClass) && string.IsNullOrEmpty(CurrentPageButtonStyle))
                    {
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "red");
                    }
                    else
                        AddClassAndStyle(CurrentPageButtonClass, CurrentPageButtonStyle, writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    if (!string.IsNullOrEmpty(CurrentPageButtonTextFormatString))
                        writer.Write(String.Format(CurrentPageButtonTextFormatString, index));
                    else
                        writer.Write(index);
                    writer.RenderEndTag();
                }
                else
                {
                    if (Enabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(index));
                    }
                    if (PagingButtonLayoutType == PagingButtonLayoutType.None) //add css class and style attribute to hyper link directory
                        AddClassAndStyle(PagingButtonClass, PagingButtonStyle, writer);
                    AddToolTip(writer, index);
                    AddHyperlinkTarget(writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    if (!string.IsNullOrEmpty(NumericButtonTextFormatString))
                        writer.Write(String.Format(NumericButtonTextFormatString, index));
                    else
                        writer.Write(index);
                    writer.RenderEndTag();
                }
            }
            if ((!isCurrent && PagingButtonLayoutType != PagingButtonLayoutType.None) || (isCurrent && PagingButtonLayoutType == PagingButtonLayoutType.UnorderedList))
                writer.RenderEndTag(); //</li>
        }

	    /// <summary>
        /// Create numeric image button.
        /// </summary>
        /// <param name="writer">A <see cref="System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        /// <param name="index">the page index correspond to the button.</param>
        /// <param name="isCurrent">if the page index correspond to the button is the current page index</param>
        private void CreateNumericImages(HtmlTextWriter writer, int index, bool isCurrent)
        {
            AddPagingButtonLayoutTag(writer); //<li> or <span>

            string indexStr = index.ToString();
            for (int i = 0; i < indexStr.Length; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, indexStr[i], (isCurrent) ? CpiButtonImageNameExtension : ButtonImageNameExtension, ButtonImageExtension));
                if (ButtonImageAlign != ImageAlign.NotSet)
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
            if (PagingButtonLayoutType != PagingButtonLayoutType.None)
                writer.RenderEndTag(); //</li> or </span>
        }

        /// <summary>
        /// create more (...) button.
        /// </summary>
        private void CreateMoreButton(HtmlTextWriter writer, int pageIndex)
        {
            if (!ShowMoreButtons)
                return;
            AddClassAndStyle(MoreButtonClass, MoreButtonStyle, writer);
            AddPagingButtonLayoutTag(writer); //<li> or <span>

            writeSpacingStyle(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
            AddToolTip(writer, pageIndex);
            AddHyperlinkTarget(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.A); //<a>
            if (PagingButtonType == PagingButtonType.Image && MoreButtonType == PagingButtonType.Image)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, "more", ButtonImageNameExtension, ButtonImageExtension));
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                if (ButtonImageAlign != ImageAlign.NotSet)
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Img); //<img>
                writer.RenderEndTag(); //</img>
            }
            else
                writer.Write("...");

            writer.RenderEndTag(); //</a>

            if (PagingButtonLayoutType != PagingButtonLayoutType.None)
                writer.RenderEndTag(); //</li> or </span>
        }

        /// <summary>
        /// Add paging button spacing styles to HtmlTextWriter
        /// </summary>
        private void writeSpacingStyle(HtmlTextWriter writer)
        {
            if (PagingButtonSpacing.Value != 0)
                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginRight, PagingButtonSpacing.ToString());
        }

        /// <summary>
        /// add target attribute to paging hyperlink
        /// </summary>
        private void AddHyperlinkTarget(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(UrlPagingTarget))
                writer.AddAttribute(HtmlTextWriterAttribute.Target, UrlPagingTarget);
        }

        /// <summary>
        /// add css class and style attribute to HtmlTextWriter
        /// </summary>
        private void AddClassAndStyle(string clsname, string style, HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(clsname))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, clsname);
            if (!string.IsNullOrEmpty(style))
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);
        }


	    #endregion

        #region IPostBackEventHandler Implementation

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="RaisePostBackEvent"]/*'/>
        public void RaisePostBackEvent(string args)
        {
            int pageIndex = CurrentPageIndex;
            try
            {
                if (string.IsNullOrEmpty(args))
                    args = inputPageIndex;
                pageIndex = int.Parse(args);
            }
            catch{}
            PageChangingEventArgs pcArgs = new PageChangingEventArgs(pageIndex);
            if (cloneFrom != null)
                cloneFrom.OnPageChanging(pcArgs);
            else
                OnPageChanging(pcArgs);
        }
        
	    #endregion

        #region IPostBackDataHandler Implementation

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="LoadPostData"]/*'/>
        public virtual bool LoadPostData(string pkey, NameValueCollection pcol)
        {
            string str = pcol[UniqueID + "_input"];
            if (str != null && str.Trim().Length > 0)
            {
                try
                {
                    int pindex = int.Parse(str);
                    if (pindex > 0 && pindex <= PageCount)
                    {
                        inputPageIndex = str;
                        Page.RegisterRequiresRaiseEvent(this);
                    }
                }
                catch{ }
            }
            return false;
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="RaisePostDataChangedEvent"]/*'/>
        public virtual void RaisePostDataChangedEvent() { }

        #endregion

        #region Events

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Event[@name="PageChanging"]/*'/>
        public event PageChangingEventHandler PageChanging
        {
            add
            {
                Events.AddHandler(EventPageChanging, value);
            }
            remove
            {
                Events.RemoveHandler(EventPageChanging, value);
            }
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Event[@name="PageChanged"]/*'/>
        public event EventHandler PageChanged
        {
            add
            {
                Events.AddHandler(EventPageChanged, value);
            }
            remove
            {
                Events.RemoveHandler(EventPageChanged, value);
            }
        }

        #endregion

        #region Methods

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="OnPageChanging"]/*'/>
        protected virtual void OnPageChanging(PageChangingEventArgs e)
        {
            pageChangeEventHandled = true;
            PageChangingEventHandler handler = (PageChangingEventHandler)Events[EventPageChanging];
            if (handler != null)
            {
                handler(this, e);
                if (!e.Cancel || UrlPaging) //there's no way we can obtain the last value of the CurrentPageIndex in UrlPaging mode, so it doesn't make sense to cancel PageChanging event in UrlPaging mode
                {
                    CurrentPageIndex = e.NewPageIndex;
                    OnPageChanged(EventArgs.Empty);
                }
            }
            else
            {
                CurrentPageIndex = e.NewPageIndex;
                OnPageChanged(EventArgs.Empty);
            }
            pageChangeEventHandled = false;
        }

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Method[@name="OnPageChanged"]/*'/>
        protected virtual void OnPageChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EventPageChanged];
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }

	#endregion

    #region PageChangingEventHandler Delegate
    /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Delegate[@name="PageChangingEventHandler"]/*'/>
	public delegate void PageChangingEventHandler(object src,PageChangingEventArgs e);

	#endregion

    #region PageChangingEventArgs Class
    /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Class[@name="PageChangingEventArgs"]/*'/>
	public sealed class PageChangingEventArgs:CancelEventArgs
	{
		private readonly int _newpageindex;

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Constructor[@name="PageChangingEventArgs"]/*'/>
		public PageChangingEventArgs(int newPageIndex)
		{
			_newpageindex=newPageIndex;
		}

        /// <include file='AspNetPagerDocs.xml' path='AspNetPagerDoc/Property[@name="NewPageIndex"]/*'/>
		public int NewPageIndex
		{
			get{return _newpageindex;}
		}
	}
	#endregion

}