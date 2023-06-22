using verificable.Controllers;
using verificable.Models;

namespace TestVerificable
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTotalRightPercentage()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            List<Adquirente> compraventaAdquirentes = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 1, RunRut = "2-7", PorcentajeDerecho = 20, NoAcreditado = false },
                new Adquirente { Id = 2, NumAtencion = 2, RunRut = "3-5", PorcentajeDerecho = 30, NoAcreditado = false },
            };
            double? expectedOutput = 50;
            double? actualOutput = controller.TotalRightPercentage(compraventaAdquirentes);

            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}