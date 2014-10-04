using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace MRSES.Windows.Forms
{
    public partial class FormAbout : Form
    {
        #region Variables

        AssemblyInfo _assemblyInfo;
        string _programInfo = string.Empty;
        
        #endregion

        public FormAbout()
        {
            InitializeComponent();

            _assemblyInfo = new AssemblyInfo(Assembly.GetEntryAssembly());

            _programInfo = "MR. Special Employee Schedule \n(MRSES por sus siglas en inglés)";
            _programInfo += Environment.NewLine + Environment.NewLine + "Versión: " + _assemblyInfo.Version;
            _programInfo += Environment.NewLine + _assemblyInfo.Copyright;
            _programInfo += Environment.NewLine + _assemblyInfo.Company;
            _programInfo += Environment.NewLine + _assemblyInfo.Description;
            _programInfo += Environment.NewLine + _assemblyInfo.TradeMark;
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            LabelFormAboutInformation.Text = _programInfo;
        }
    }

    public class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            this.assembly = assembly;
        }

        private readonly Assembly assembly;

        /// <summary>
        /// Gets the title property
        /// </summary>
        public string ProductTitle
        {
            get
            {
                return GetAttributeValue<AssemblyTitleAttribute>(a => a.Title,
                       System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase));
            }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version
        {
            get
            {
                string result = string.Empty;
                Version version = assembly.GetName().Version;
                if (version != null)
                    return version.ToString();
                else
                    return "1.0.0.0";
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<AssemblyDescriptionAttribute>(description => description.Description); }
        }


        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return GetAttributeValue<AssemblyProductAttribute>(product => product.Product); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return GetAttributeValue<AssemblyCopyrightAttribute>(copyright => copyright.Copyright); }
        }

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company
        {
            get { return GetAttributeValue<AssemblyCompanyAttribute>(company => company.Company); }
        }

        public string TradeMark
        {
            get { return GetAttributeValue<AssemblyTrademarkAttribute>(tradeMark => tradeMark.Trademark); }
        }

        protected string GetAttributeValue<TAttr>(Func<TAttr,
          string> resolveFunc, string defaultResult = null) where TAttr : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
                return resolveFunc((TAttr)attributes[0]);
            else
                return defaultResult;
        }
    }
}
