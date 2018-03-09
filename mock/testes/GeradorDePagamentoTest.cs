using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mock.dominio;
using mock.servico;
using NUnit.Framework;
using Moq;
using mock.infra;

namespace mock.testes
{
    [TestFixture]
    public class GeradorDePagamentoTest
    {
        [Test]
        public void DeveGerarPagamentoParaLeilaoEncerrado()
        {
            var leilaoDao = new Mock<LeilaoDaoFalso>();
            var avaliador = new Avaliador();
            var pagamentoDao = new Mock<IPagamentoDao>();

            var leilao1 = new Leilao("Playstation");
            leilao1.naData(new DateTime(1999, 5, 5));
            leilao1.propoe(new Lance(new Usuario("renan"), 500));
            leilao1.propoe(new Lance(new Usuario("Felipe"), 600));
            var listaDeLeiloes = new List<Leilao> { leilao1 };
            
            leilaoDao.Setup(l => l.encerrados()).Returns(listaDeLeiloes);

            Pagamento pagamentoCapturado = null;
            pagamentoDao.Setup(p => p.Salvar(It.IsAny<Pagamento>())).Callback<Pagamento>(c => pagamentoCapturado = c);

            var gerador = new GeradorDePagamento(leilaoDao.Object,avaliador,pagamentoDao.Object);
            gerador.gera();

            Assert.AreEqual(600, pagamentoCapturado.valor);
        }

        [Test]
        public void DeveJogarParaOProximoDiaUtil()
        {
            var leilaoDao = new Mock<LeilaoDaoFalso>();
            var avaliador = new Avaliador();
            var pagamentoDao = new Mock<IPagamentoDao>();
            var relogio = new Mock<IRelogio>();

            relogio.Setup(r => r.hoje()).Returns(new DateTime(2012, 4, 7));
            var leilao1 = new Leilao("Playstation");
            leilao1.naData(new DateTime(1999, 5, 5));
            leilao1.propoe(new Lance(new Usuario("renan"), 500));
            leilao1.propoe(new Lance(new Usuario("Felipe"), 600));
            var listaDeLeiloes = new List<Leilao> { leilao1 };

            leilaoDao.Setup(l => l.encerrados()).Returns(listaDeLeiloes);

            Pagamento pagamentoCapturado = null;
            pagamentoDao.Setup(p => p.Salvar(It.IsAny<Pagamento>())).Callback<Pagamento>(c => pagamentoCapturado = c);

            var gerador = new GeradorDePagamento(leilaoDao.Object, avaliador, pagamentoDao.Object,relogio.Object);
            gerador.gera();

            Assert.AreEqual(DayOfWeek.Monday, pagamentoCapturado.data.DayOfWeek);
        }

        [Test]
        public void DeveJogarParaOProximoDiaUtilCasoDomingo()
        {
            var leilaoDao = new Mock<LeilaoDaoFalso>();
            var avaliador = new Avaliador();
            var pagamentoDao = new Mock<IPagamentoDao>();
            var relogio = new Mock<IRelogio>();

            relogio.Setup(r => r.hoje()).Returns(new DateTime(2018, 3, 11));
            var leilao1 = new Leilao("Playstation");
            leilao1.naData(new DateTime(1999, 5, 5));
            leilao1.propoe(new Lance(new Usuario("renan"), 500));
            leilao1.propoe(new Lance(new Usuario("Felipe"), 600));
            var listaDeLeiloes = new List<Leilao> { leilao1 };

            leilaoDao.Setup(l => l.encerrados()).Returns(listaDeLeiloes);

            Pagamento pagamentoCapturado = null;
            pagamentoDao.Setup(p => p.Salvar(It.IsAny<Pagamento>())).Callback<Pagamento>(c => pagamentoCapturado = c);

            var gerador = new GeradorDePagamento(leilaoDao.Object, avaliador, pagamentoDao.Object, relogio.Object);
            gerador.gera();

            Assert.AreEqual(DayOfWeek.Monday, pagamentoCapturado.data.DayOfWeek);
        }
    }
}
