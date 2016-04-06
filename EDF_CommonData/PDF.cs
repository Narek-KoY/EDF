using System;
using System.IO;
using Microsoft.SharePoint;
using iTextSharp.text.pdf;

namespace EDF_CommonData
{
    public static class PDF
    {
        public static string SaveInSP(string filePath, string autorId, string requestId)
        {
            string fileToUpload = filePath;
            string folderName = "PDFFiles";
            //string fileName = filePath.Split('\\')[filePath.Split('\\').ToList().Count - 1];

            string url = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPSite site = new SPSite(SPContext.Current.Web.Url);
                SPWeb oWeb = site.OpenWeb();
                oWeb.AllowUnsafeUpdates = true;

                if (!System.IO.File.Exists(fileToUpload))
                    throw new FileNotFoundException("File not found.", fileToUpload);

                SPFolder myLibrary = oWeb.Folders[folderName];

                bool b = true;
                foreach (SPFile file in myLibrary.Files)
                {
                    if (file.GetProperty("Request Id").ToString().Equals(requestId, StringComparison.InvariantCulture))
                    {
                        url = file.Url;
                        file.Delete();
                        //b = false;
                        break;
                    }
                }

                myLibrary.Update();

                if (b)
                {
                    // Prepare to upload
                    Boolean replaceExistingFiles = true;
                    String fileName = System.IO.Path.GetFileName(fileToUpload);
                    FileStream fileStream = File.OpenRead(fileToUpload);

                    // Upload document
                    SPFile spfile = myLibrary.Files.Add(requestId.ToString() + ".pdf", fileStream, replaceExistingFiles);
                    spfile.AddProperty("Autor Name", autorId);
                    spfile.AddProperty("Request Id", requestId);
                    spfile.Update();
                    // Commit 
                    myLibrary.Update();

                    url = SPContext.Current.Site.Url + "/" + myLibrary.Url + "/" + fileName;

                    fileStream.Close();
                }
            });

            return url;
        }

        public static string SaveDARFileInSP(string filePath, string requestId)
        {
            String fileToUpload = filePath;
            String folderName = "DARFiles";
            //string FileName = filePath.Split('\\')[filePath.Split('\\').ToList().Count - 1];

            string url = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPSite site = new SPSite(SPContext.Current.Web.Url);
                SPWeb oWeb = site.OpenWeb();
                oWeb.AllowUnsafeUpdates = true;

                if (!System.IO.File.Exists(fileToUpload))
                    throw new FileNotFoundException("File not found.", fileToUpload);

                SPFolder myLibrary = oWeb.Folders[folderName];

                bool b = true;
                foreach (SPFile file in myLibrary.Files)
                {
                    if (file.GetProperty("Request Id").ToString() == requestId)
                    {
                        url = file.Url;
                        b = false;
                        break;
                    }
                }

                if (b)
                {
                    // Prepare to upload
                    Boolean replaceExistingFiles = true;
                    String fileName = System.IO.Path.GetFileName(fileToUpload);
                    FileStream fileStream = File.OpenRead(fileToUpload);

                    // Upload document
                    SPFile spfile = myLibrary.Files.Add(fileName, fileStream, replaceExistingFiles);
                    spfile.AddProperty("Request Id", requestId);
                    spfile.Update();
                    // Commit 
                    myLibrary.Update();

                    url = SPContext.Current.Site.Url + "/" + myLibrary.Url + "/" + fileName;
                }
            });

            return url;
        }

        public static string GetDARFile(string requestId)
        {
            String folderName = "DARFiles";
            string url = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPSite site = new SPSite(SPContext.Current.Web.Url);
                SPWeb oWeb = site.OpenWeb();

                SPFolder myLibrary = oWeb.Folders[folderName];

                // Upload document
                foreach (SPFile file in myLibrary.Files)
                {
                    if (file.GetProperty("Request Id").ToString() == requestId)
                    {
                        url = SPContext.Current.Site.Url + "/" + file.Url;
                        break;
                    }
                }
            });

            return url;
        }

        public static string GetFromSPFolder(string requestId)
        {
            String folderName = "PDFFiles";
            string url = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPSite site = new SPSite(SPContext.Current.Web.Url);
                SPWeb oWeb = site.OpenWeb();

                SPFolder myLibrary = oWeb.Folders[folderName];

                // Upload document
                foreach (SPFile file in myLibrary.Files)
                {
                    if (file.GetProperty("Request Id").ToString() == requestId)
                    {
                        url = SPContext.Current.Site.Url + "/" + file.Url;
                        break;
                    }
                }
            });

            return url;
        }

        public static bool hasSertificatedFile(string requestId)
        {
            return !string.IsNullOrEmpty(GetFromSPFolder(requestId));
        }
    }

    public class PdfDoc
    {
        PdfReader pr;
        public PdfDoc(string filePath)
        {
            pr = new PdfReader(filePath);
        }

        public bool HasCertificate
        {
            get
            {
                AcroFields af = pr.AcroFields;
                return !(af.GetSignatureNames().Count == 0);
            }
        }

        public void close()
        {
            pr.Close();
        }
    }
}
