using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.PageObjects;

namespace LoremIpsumProject.Tests
{
    public class BaseTest
    {
        IWebDriver driver;

        string test_url = "https://lipsum.com/";

        [FindsBy(How = How.XPath, Using = "//a[@class='ru']")]
        private IWebElement russian_button;

        [FindsBy(How = How.CssSelector, Using = "#Panes > div:nth-child(1) > p")]
        private IWebElement first_paragraph;

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver();
            PageFactory.InitElements(driver, this);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void CheckTheWordAppearsCorrectly(int num)
        {
            Console.WriteLine(num);
            driver.Navigate().GoToUrl(test_url);
            russian_button.Click();
            Assert.IsTrue(first_paragraph.Text.Contains("рыба"));
        }


        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
