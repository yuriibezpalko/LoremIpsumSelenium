using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        [FindsBy(How = How.XPath, Using = "//label[@for='bytes']")]
        private IWebElement bytes;

        [FindsBy(How = How.XPath, Using = "//input[@type='checkbox']")]
        private IWebElement checkbox;

        [FindsBy(How = How.XPath, Using = "//*[@id='lipsum']/p")]
        private IWebElement paragraphs_list;

        [FindsBy(How = How.CssSelector, Using = "#lipsum > p:nth-child(1)")]
        private IWebElement starting_text;

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
            WaitForElement(first_element_text, wait);
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
        public void CheckThatGeneratedWordSizeIsCorrect(int value)
        {
            int length;
            if (value <= 0)
            {
                length = value;
            }
            else
            {
                words.Click();
                number_field.Clear();
                number_field.SendKeys(Convert.ToString(value));
                generate_button.Click();
                WaitForElement(number_of_words, wait);
                length = number_of_words.Text.Trim(new char[] { ',', '.' }).Split(" ").Length;
            }
            Assert.AreEqual(value, length);
        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(0)]
        [TestCase(1)]
        public void CheckThatGeneratedByteSizeIsCorrect(int value)
        {
            int length;
            if (value < 3)
            {
                length = value;
            }
            else
            {
                bytes.Click();
                number_field.Clear();
                number_field.SendKeys(Convert.ToString(value));
                generate_button.Click();
                WaitForElement(number_of_words, wait);
                length = number_of_words.Text.ToCharArray().Length;
            }
            Assert.AreEqual(value, length);
        }

        public void CheckboxVerifying()
        {
            checkbox.Click();
            generate_button.Click();
            Assert.IsFalse(starting_text.Text.StartsWith("Lorem ipsum"));
        }

        [TestCase("lorem", 2)]
        [TestCase("ipsum", 100)]
        public void CheckThatParagraphTextContainsSearchingWord(string searchTerm, int num_of_itertions = 10)
        {
            int all_word_matches = 0;
            ReadOnlyCollection<IWebElement> elementTexts;

            for (int i = 0; i < num_of_itertions; i++)
            {
                generate_button.Click();
                elementTexts = driver.FindElements(By.XPath("//*[@id='lipsum']/p"));

                all_word_matches += elementTexts
                    .Select(x => x.Text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => Array.FindAll(x, y => y.ToLower() == searchTerm).Count()).Sum();
                driver.Navigate().GoToUrl(test_url);
            }

            int average = all_word_matches / num_of_itertions;
            Assert.IsTrue(Enumerable.Range(2, 3).Contains(average), $"Expected to be {true}, but got {average}");
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
