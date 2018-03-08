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
            var leilao1 = new Leilao("Playstation");
            leilao1.naData(new DateTime(1999, 5, 5));

            leilao1.propoe(new Lance(new Usuario("renan"), 500));
            leilao1.propoe(new Lance(new Usuario("Felipe"), 600));

            var listaDeLeiloes = new List<Leilao> { leilao1 };
            var leilaoDao = new Mock<LeilaoDaoFalso>();
            leilaoDao.Setup(l => l.encerrados()).Returns(listaDeLeiloes);

            var avaliador = new Mock<Avaliador>();
            avaliador.Setup(a => a.maiorValor).Returns(600);

            var pagamentoDao = new Mock<IPagamentoDao>();

            Pagamento pagamentoCapturado = null;
            pagamentoDao.Setup(p => p.Salvar(It.IsAny<Pagamento>())).Callback<Pagamento>(c => pagamentoCapturado = c);

            var gerador = new GeradorDePagamento(leilaoDao.Object,avaliador.Object,pagamentoDao.Object);
            gerador.gera();

            Assert.AreEqual(600, pagamentoCapturado.valor);
        }
    }
}
