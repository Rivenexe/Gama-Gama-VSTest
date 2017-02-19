using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenPop.Pop3;
//using OpenPop.Mime;
using OpenPop.Mime.Header;


namespace Gama_Gama_VSTest
{
    [TestClass]
    public class LoginForm
    {
        const string gg = "http://gama-gama.ru/";
        const string wrongEmail = "testemail";
        const string wrongPassword = "testpass";
        const string validEmail = "sharapov.gama-gama_test@india.com";
        const string validPassword = "sharapovTSTpass";
        const string hostname = "pop.india.com";
        const int port = 995;
        const bool useSsl = true;
        const string emailPass = "gamagamaTST";

        public static bool IsStale(IWebElement element)
        {
            try
            {
                element.Click();
                return false;
            }
            catch (StaleElementReferenceException sere)
            {
                return true;
            }
        }

        /// <summary>
        /// Открытие и закрытие формы
        /// </summary>
        [TestMethod]
        public void LoginForm_OpenClose()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                //Открытие
                formLink.Click();
                IWebElement closeButton = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cboxClose")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Закрытие по кнопке [X]
                closeButton.Click();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("cboxClose")));

                Assert.IsTrue(!colorbox.Displayed, "Форма не закрылась");
                Assert.IsTrue(!overlay.Displayed, "Оверлей не пропал");

                formLink.Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cboxClose")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась повторно");

                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", overlay); ///overlay.Click() не работает на оверлее
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("cboxClose")));

                Assert.IsTrue(!colorbox.Displayed, "Форма не закрылась повторно");
                Assert.IsTrue(!overlay.Displayed, "Оверлей не пропал повторно");
            }
        }

        /// <summary>
        /// Попытка авторизации пароля с пустым email и паролем
        /// </summary>
        [TestMethod]
        public void LoginForm_LoginWithEmptyEmailAndPassword()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button"))); 

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                loginButton.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Введите E-mail, на который вы регистрировались")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };
                
                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");
            }
        }

        /// <summary>
        /// Попытка авторизации с пустым паролем
        /// </summary>
        [TestMethod]
        public void LoginForm_LoginWithEmptyPassword()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button")));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Ввод реквизитов
                emailField.SendKeys(wrongEmail);
                loginButton.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Введите ваш пароль")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };

                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");
            }
        }

        /// <summary>
        /// Попытка авторизации с пустым email
        /// </summary>
        [TestMethod]
        public void LoginForm_LoginWithEmptyEmail()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button")));
                IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Password")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Ввод реквизитов
                passwordField.SendKeys(wrongPassword);
                loginButton.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Введите E-mail, на который вы регистрировались")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };

                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");
            }
        }

        /// <summary>
        /// Попытка авторизации с неправильным паролем
        /// </summary>
        [TestMethod]
        public void LoginForm_LoginWithWrongPassword()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button")));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));
                IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Password")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Ввод реквизитов
                emailField.SendKeys(validEmail);
                passwordField.SendKeys(wrongPassword);
                loginButton.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Неправильный пароль.")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };

                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");

                //Восстановление пароля;
                IWebElement restorePasswordLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[2]/a")));
                restorePasswordLink.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.ClassName("status-message"), "Инструкция по восстановлению отправлена")); }    
                catch (Exception e) { Assert.Fail("Не появилось сообщение об отправке письма"); };
            }
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(validEmail, emailPass);

                int messageNumber = client.GetMessageCount();
                int i=0;
                while (messageNumber == 0 && i < 60)
                {
                    client.Disconnect();
                    System.Threading.Thread.Sleep(5000);
                    client.Connect(hostname, port, useSsl);
                    client.Authenticate(validEmail, emailPass);
                    messageNumber = client.GetMessageCount();
                    i = i++;
                }
                 
                Assert.IsTrue(messageNumber > 0, "Письмо не пришло");

                MessageHeader headers = client.GetMessageHeaders(messageNumber);
                RfcMailAddress from = headers.From;
                string subject = headers.Subject;
                client.DeleteAllMessages();

                Assert.IsFalse(from.HasValidMailAddress && from.Address.Equals("help@gama-gama.ru") && "Восстановление пароля на Gama - Gama".Equals(subject), "Письмо не пришло");
            }
            
        }

        /// <summary>
        /// Корректная авторизация
        /// </summary>
        [TestMethod]
        public void LoginForm_ValidLogin()
        {
            FirefoxProfile fireFoxP = new FirefoxProfile("SeleniumProfile",false);

            using (IWebDriver driver = new FirefoxDriver(fireFoxP))
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button")));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));
                IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Password")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Ввод реквизитов
                emailField.SendKeys(validEmail);
                passwordField.SendKeys(validPassword);
                loginButton.Click();
                try { wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("Личный кабинет"))); }
                catch (Exception e) { Assert.Fail("Авторизация не прошла"); }

                Assert.IsTrue(IsStale(colorbox), "Форма не закрылась");
                Assert.IsTrue(IsStale(overlay), "Оверлей не пропал");
            }

            //Переоткрытие браузера
            using (IWebDriver driver = new FirefoxDriver(fireFoxP))
            {
                driver.Navigate().GoToUrl(gg);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("Личный кабинет")));
                    Assert.Fail("Авторизация сохраняется");
                }
                catch (Exception e) { }              
            }
        }

        /// <summary>
        /// Корректная авторизация c опцией "запомнить меня"
        /// </summary>
        [TestMethod]
        public void LoginForm_ValidPersistentLogin()
        {
            FirefoxProfile fireFoxP = new FirefoxProfile("SeleniumProfile", false);

            using (IWebDriver driver = new FirefoxDriver(fireFoxP))
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement persistentCheckbox = driver.FindElement(By.ClassName("auth_form_stay_signed"));
                IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""authblock""]/div[2]/form/div[7]/button")));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));
                IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Password")));


                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                //Ввод реквизитов
                emailField.SendKeys(validEmail);
                passwordField.SendKeys(validPassword);
                persistentCheckbox.Click();
                loginButton.Click();
                try { wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("Личный кабинет"))); }
                catch (Exception e) { Assert.Fail("Авторизация не прошла"); }

                Assert.IsTrue(IsStale(colorbox), "Форма не закрылась");
                Assert.IsTrue(IsStale(overlay), "Оверлей не пропал");
            }

            //Переоткрытие браузера
            using (IWebDriver driver = new FirefoxDriver(fireFoxP))
            {
                driver.Navigate().GoToUrl(gg);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("Личный кабинет")));
                }
                catch (Exception e) { Assert.Fail("Авторизация не сохраняется"); }
            }
        }

        /// <summary>
        /// Попытка восстановления пароля с пустым email
        /// </summary>
        [TestMethod]
        public void LoginForm_PasswordRestoreWithEmptyEmail()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement restoreLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Я не помню пароль")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                restoreLink.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Введите E-mail, на который вы регистрировались")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };

                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");
            }
        }

        /// <summary>
        /// Попытка восстановления пароля с неправильным email
        /// </summary>
        [TestMethod]
        public void LoginForm_PasswordRestoreWithWrongEmail()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));
                IWebElement restoreLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Я не помню пароль")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                emailField.SendKeys(wrongEmail);
                restoreLink.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElement(messageBox, "Введен неверный Email")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об ошибке"); };

                Assert.IsTrue(colorbox.Displayed, "Форма закрылась");
                Assert.IsTrue(overlay.Displayed, "Оверлей пропал");
            }
        }

        /// <summary>
        /// Попытка восстановления пароля с корректным email
        /// </summary>
        [TestMethod]
        public void LoginForm_PasswordRestoreWithValidEmail()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(gg);

                //Открытие формы
                IWebElement formLink = driver.FindElement(By.XPath(@"//*[@id=""top_back""]/div[3]/a[1]"));
                formLink.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement overlay = driver.FindElement(By.Id("cboxOverlay"));
                IWebElement colorbox = driver.FindElement(By.Id("colorbox"));
                IWebElement messageBox = driver.FindElement(By.ClassName("status-error"));
                IWebElement emailField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Email")));
                IWebElement restoreLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Я не помню пароль")));

                Assert.IsTrue(colorbox.Displayed, "Форма не открылась");

                emailField.SendKeys(validEmail);
                restoreLink.Click();
                try { wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.ClassName("status-message"), "Инструкция по восстановлению отправлена")); }
                catch (Exception e) { Assert.Fail("Не появилось сообщение об отправке письма"); }
            }
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(validEmail, emailPass);

                int messageNumber = client.GetMessageCount();
                int i = 0;
                while (messageNumber == 0 && i < 60)
                {
                    client.Disconnect();
                    System.Threading.Thread.Sleep(5000);
                    client.Connect(hostname, port, useSsl);
                    client.Authenticate(validEmail, emailPass);
                    messageNumber = client.GetMessageCount();
                    i = i++;
                }

                Assert.IsTrue(messageNumber > 0, "Письмо не пришло");

                MessageHeader headers = client.GetMessageHeaders(messageNumber);
                RfcMailAddress from = headers.From;
                string subject = headers.Subject;
                client.DeleteAllMessages();

                Assert.IsFalse(from.HasValidMailAddress && from.Address.Equals("help@gama-gama.ru") && "Восстановление пароля на Gama - Gama".Equals(subject), "Письмо не пришло");
            }
        }

    }
}
