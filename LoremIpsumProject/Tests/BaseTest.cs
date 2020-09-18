using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace LoremIpsumProject.Tests
{
    public class BaseTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private readonly string test_url = "https://lipsum.com/";

        [FindsBy(How = How.XPath, Using = "//a[@class='ru']")]
        private IWebElement russian_button;

        [FindsBy(How = How.CssSelector, Using = "#Panes > div:nth-child(1) > p")]
        private IWebElement first_element_text;

        [FindsBy(How = How.XPath, Using = "//input[@id='generate']")]
        private IWebElement generate_button;

        [FindsBy(How = How.CssSelector, Using = "#lipsum > p:nth-child(1)")]
        private IWebElement first_paragraph;

        [FindsBy(How = How.XPath, Using = "//label[@for='words']")]
        private IWebElement words;

        [FindsBy(How = How.XPath, Using = "//input[@type='text']")]
        private IWebElement number_field;

        [FindsBy(How = How.Id, Using = "lipsum")]
        private IWebElement number_of_words;

        private readonly long timeout = 3000;


        public void WaitForPageLoadComplete()
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void WaitForElement(IWebElement element, WebDriverWait wait, int timeout = 2)
        {
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            wait.Until<bool>(driver =>
            {
                try
                {
                    return element.Displayed;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl(test_url);
            PageFactory.InitElements(driver, this);
        }

        [Test]
        public void CheckTheWordAppearsCorrectly()
        {
            russian_button.Click();
            WaitForPageLoadComplete();
            Assert.IsTrue(first_element_text.Text.Contains("рыба"));
        }

        [Test]
        public void CheckTheFirstParagraphStartsCorrectly()
        {
            generate_button.Click();
            WaitForPageLoadComplete();
            Assert.IsTrue(first_paragraph.Text.Contains("Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
        }

        [TestCase(20)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(10)]
        public void CheckThatGeneratedSizeCorrect(int value)
        {
            words.Click();
            number_field.Clear();
            number_field.SendKeys(Convert.ToString(value));
            generate_button.Click();
            WaitForElement(number_of_words, wait);
            string for_test = number_of_words.Text;
            int length = number_of_words.Text.Trim(new char[] { ',', '.' }).Split(" ").Length;
            Assert.AreEqual(value, length);
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
