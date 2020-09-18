using System;
using System.Diagnostics;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace LoremIpsumProject.Tests
{
    public class BaseTest
    {
        IWebDriver driver;
        private readonly string test_url = "https://lipsum.com/";

        [FindsBy(How = How.XPath, Using = "//a[@class='ru']")]
        private readonly IWebElement russian_button;

        [FindsBy(How = How.CssSelector, Using = "#Panes > div:nth-child(1) > p")]
        private readonly IWebElement first_element_text;

        [FindsBy(How = How.XPath, Using = "//input[@id='generate']")]
        private readonly IWebElement generate_button;

        [FindsBy(How = How.CssSelector, Using = "#lipsum > p:nth-child(1)")]
        private readonly IWebElement first_paragraph;

        [FindsBy(How = How.XPath, Using = "//label[@for='words']")]
        private readonly IWebElement words;

        [FindsBy(How = How.XPath, Using = "//input[@type='text']")]
        private readonly IWebElement number_field;

        [FindsBy(How = How.XPath, Using = "//div[@id='lipsum']")]
        private readonly IWebElement number_of_words;

        private readonly long timeout = 3000;

        public void ImplicitWait()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(timeout);
        }

        public void WaitForPageLoadComplete()
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void WaitVisibilityOfElements(IWebElement element)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                if (element.Displayed)
                {
                    return;
                }
            } while (stopwatch.ElapsedMilliseconds < timeout);
            throw new Exception("element not found");
        }

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver();
            PageFactory.InitElements(driver, this);
        }

        [Test]
        public void CheckTheWordAppearsCorrectly()
        {
            driver.Navigate().GoToUrl(test_url);
            russian_button.Click();
            WaitForPageLoadComplete();
            Assert.IsTrue(first_element_text.Text.Contains("рыба"));
        }

        [Test]
        public void CheckTheFirstParagraphStartsCorrectly()
        {
            driver.Navigate().GoToUrl(test_url);
            generate_button.Click();
            WaitForPageLoadComplete();
            Assert.IsTrue(first_paragraph.Text.Contains("Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
        }

        [TestCase(10)]
        [TestCase (-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(20)]
        public void CheckThatGeneratedSizeCorrect(int value)
        {
            driver.Navigate().GoToUrl(test_url);
            words.Click();
            number_field.Clear();
            number_field.SendKeys(Convert.ToString(value));
            generate_button.Click();
            WaitVisibilityOfElements(number_of_words);
            int length = number_of_words.Text.Trim(new char[] { ',', '.' }).Split(new char[] { ' ' }).Length;
            Assert.AreEqual(value, length);
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
