using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace NCRQuality_Inspection
{
    class SVN_API
    {      
        public SVN_API()
        {
            AHUTOR = this.AHUTOR;
            REVISION_TIME = this.REVISION_TIME;
            REVISION = this.REVISION;
        }

        public string AHUTOR { get; set; }
        public string REVISION_TIME { get; set; }
        public string REVISION { get; set; }

        public static void SVN_CHECKOUT()
        {
            DeskNotify deskNotify;
            SvnUpdateResult result;
            SvnUriTarget url = new SvnUriTarget("file://mxchim0pangea01/REPO_CosmeticScript/REPO/trunk/NCR_Quality_Inspection");
            string local = @"C:\Trunk\NCR_Quality_Inspection";

            using (SvnClient client = new SvnClient())
            {
                try
                {
                    client.CheckOut(url, local, out result);//.Update(path,updateArgs,out result);

                    if (result != null)
                    {
                        deskNotify = new DeskNotify("SVN CHECKOUT .... ", "SUCCESS REVISION: " + result.Revision, DeskNotify.NotifyType.OnSVNSucces);
                        deskNotify.Show();
                    }
                    else
                    {
                        deskNotify = new DeskNotify("SVN CHECKOUT .... ", "ERROR: " + result.Revision, DeskNotify.NotifyType.OnSVNError);
                        deskNotify.Show();
                    }
                }
                catch (SvnException svnException)
                {
                    deskNotify = new DeskNotify("SVN CHECKOUT .... ", svnException.ToString().ToUpper(), DeskNotify.NotifyType.OnSVNError);
                    deskNotify.Show();
                }
                catch (UriFormatException uriException)
                {
                    deskNotify = new DeskNotify("SVN CHECKOUT .... ", uriException.ToString().ToUpper(), DeskNotify.NotifyType.OnSVNError);
                    deskNotify.Show();
                }
            }
        }

        public void SVN_GET_REVISION()
        {
            DeskNotify deskNotify;
            string _REVISON = string.Empty;
            SvnUriTarget url = new SvnUriTarget("file://mxchim0pangea01/REPO_CosmeticScript/REPO/trunk/NCR_Quality_Inspection");
            string local = @"C:\Trunk\NCR_Quality_Inspection";

            using (SvnClient client = new SvnClient())
            {
                try
                {
                    SvnInfoEventArgs result;
                    client.GetInfo(url, out result);//.Update(path,updateArgs,out result);

                    if (result != null)
                    {
                        AHUTOR = result.LastChangeAuthor;
                        REVISION_TIME = result.LastChangeTime.ToString();
                        REVISION = result.LastChangeRevision.ToString();
                    }
                    else
                    {
                        deskNotify = new DeskNotify("SVN GET REVISION .... ", "ERROR: " + result.Revision, DeskNotify.NotifyType.OnSVNError);
                        deskNotify.Show();
                    }
                }
                catch (SvnException svnException)
                {
                    deskNotify = new DeskNotify("SVN GET REVISION .... ", svnException.ToString().ToUpper(), DeskNotify.NotifyType.OnSVNError);
                    deskNotify.Show();
                }
                catch (UriFormatException uriException)
                {
                    deskNotify = new DeskNotify("SVN GET REVISION .... ", uriException.ToString().ToUpper(), DeskNotify.NotifyType.OnSVNError);
                    deskNotify.Show();
                }
            }
        }


    }
}
