using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbandoTodo;
using ProbandoTodo.Controllers;

namespace ProbandoTodo.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        // Testing de todas las vistas

        // Tests de HomeController

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }        


        // Tests del NoteController

        [TestMethod]
        public void TestCreateNoteView()
        {
            NoteController controller = new NoteController();
            ViewResult view = controller.Create() as ViewResult;
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void TestAllNotesView()
        {
            NoteController controller = new NoteController();
            ViewResult view = controller.List() as ViewResult;
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void TestAllFoldersView()
        {
            FolderController controller = new FolderController();
            ViewResult view = controller.List() as ViewResult;
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void TestChangeFolderPartial()
        {
            FolderController controller = new FolderController();
            PartialViewResult partial = controller.ChangeFolderPartial(2, "Ocio", 3) as PartialViewResult;
            Assert.IsNotNull(partial);
        }

        [TestMethod]
        public void TestCreateFolderPartial()
        {
            FolderController controller = new FolderController();
            PartialViewResult partial = controller.Create() as PartialViewResult;
            Assert.IsNotNull(partial);
        }        
    }
}
