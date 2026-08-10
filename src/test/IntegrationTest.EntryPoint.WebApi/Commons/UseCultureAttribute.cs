using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit.v3;

namespace IntegrationTest.EntryPoint.WebApi.Commons;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly string _cultureName;
        private readonly string _uiCultureName;
        
        private CultureInfo _originalCulture;
        private CultureInfo _originalUiCulture;

        public UseCultureAttribute(string culture) : this(culture, culture) { }

        public UseCultureAttribute(string culture, string uiCulture)
        {
            _cultureName = culture;
            _uiCultureName = uiCulture;
        }

        public void Before(MethodInfo methodUnderTest)
        {
            Before(methodUnderTest, null);
        }

        public void After(MethodInfo methodUnderTest)
        {
            After(methodUnderTest, null);
        }

        public override void Before(MethodInfo methodUnderTest, IXunitTest test)
        {
            _originalCulture = Thread.CurrentThread.CurrentCulture;
            _originalUiCulture = Thread.CurrentThread.CurrentUICulture;

            SetThreadCultures(_cultureName, _uiCultureName);
        }

        public override void After(MethodInfo methodUnderTest, IXunitTest test)
        {
            SetThreadCultures(_originalCulture.Name, _originalUiCulture.Name);
        }

        private static void SetThreadCultures(string culture, string uiCulture)
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture, false);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture, false);
            }
            
            if (string.IsNullOrWhiteSpace(uiCulture)) return;
            CultureInfo.CurrentUICulture = new CultureInfo(uiCulture, false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(uiCulture, false);
    }
}
