using mock.dominio;
using mock.servico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;
using mock.infra;

namespace mock.testes
{
    [TestFixture]
    public class EncerradorDeLeilaoTest
    {
        [Test]
        public void DeveEncerrarLeiloesQueComecaramAUmaSemana()
        {
            DateTime diaDaSemanaPassada = new DateTime(1999, 5, 5);

            var leilao1 = new Leilao("Tv de plasma");
            leilao1.naData(diaDaSemanaPassada);
            var leilao2 = new Leilao("Playstation");
            leilao2.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao1, leilao2 };

            var dao = new Mock<IRepositorioDeLeiloes>();
             dao.Setup(d => d.correntes())
                .Returns(ListaDeLeioloes);

            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(2, encerrador.total);
            Assert.IsTrue(leilao1.encerrado);
            Assert.IsTrue(leilao2.encerrado);

        }

        [Test]
        public void NaoDeveEncerrarLeilaoQueComecouHoje()
        {
            var diaDaSemanaPassada = DateTime.Now;

            var leilao = new Leilao("Tv de plasma");
            leilao.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao };

            var daoFalso = new Mock<IRepositorioDeLeiloes>();
            daoFalso.Setup(metodo => metodo.correntes())
                .Returns(ListaDeLeioloes);
            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(daoFalso.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
            Assert.IsFalse(leilao.encerrado);
        }

        [Test]
        public void EncerradorNaoDeveFazerNada()
        {
            var daoFalso = new Mock<IRepositorioDeLeiloes>();
            daoFalso.Setup(m => m.correntes()).Returns(new List<Leilao>());
            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(daoFalso.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
            
        }

        [Test]
        public void DeveEncerrarOsLEiloesESalvarNoDao()
        {
            var diaDaSemanaPassada = new DateTime(1999, 5, 5);

            var leilao1 = new Leilao("Tv de plasma");
            leilao1.naData(diaDaSemanaPassada);
            var leilao2 = new Leilao("Playstation");
            leilao2.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao1, leilao2 };

            var dao = new Mock<IRepositorioDeLeiloes>();
            dao.Setup(d => d.correntes())
               .Returns(ListaDeLeioloes);

            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(v => v.atualiza(leilao1),Times.Once);
            dao.Verify(v => v.atualiza(leilao2), Times.Once);
        }

        [Test]
        public void NaoDeveAtualizaOsLeiloesEncerrados()
        {
            DateTime data = DateTime.Now;

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(v => v.atualiza(leilao1), Times.Never);

        }

        [Test]
        public void DeveContinuarMesmoQuandoLancaExcecao()
        {
            var diaDaSemanaPassada = new DateTime(1999, 5, 5);

            var leilao1 = new Leilao("Tv de plasma");
            leilao1.naData(diaDaSemanaPassada);
            var leilao2 = new Leilao("Playstation");
            leilao2.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao1, leilao2 };

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(d => d.correntes()).Returns(ListaDeLeioloes);
            dao.Setup(d => d.atualiza(leilao1)).Throws(new Exception());
            var carteiro = new Mock<ICarteiro>();

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(d => d.atualiza(leilao2), Times.Once);
            carteiro.Verify(c => c.envia(leilao2), Times.Once);
            carteiro.Verify(c => c.envia(leilao1), Times.Never());
        }

        [Test]
        public void DeveContinuarMesmoQuandoLancaExcecaoNoCarteiro()
        {
            var diaDaSemanaPassada = new DateTime(1999, 5, 5);

            var leilao1 = new Leilao("Tv de plasma");
            leilao1.naData(diaDaSemanaPassada);
            var leilao2 = new Leilao("Playstation");
            leilao2.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao1, leilao2 };

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(d => d.correntes()).Returns(ListaDeLeioloes);
            var carteiro = new Mock<ICarteiro>();
            carteiro.Setup(c => c.envia(leilao1)).Throws(new Exception());
            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(d => d.atualiza(leilao2), Times.Once);
            carteiro.Verify(c => c.envia(leilao2), Times.Once);
        }

        [Test]
        public void NaoDeveEnviarEmailCasoTodasChamadasDoAtualizaLancemExcecoes()
        {
            var diaDaSemanaPassada = new DateTime(1999, 5, 5);

            var leilao1 = new Leilao("Tv de plasma");
            leilao1.naData(diaDaSemanaPassada);
            var leilao2 = new Leilao("Playstation");
            leilao2.naData(diaDaSemanaPassada);
            var leilao3 = new Leilao("Playstation");
            leilao3.naData(diaDaSemanaPassada);

            var ListaDeLeioloes = new List<Leilao> { leilao1, leilao2,leilao3 };

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(d => d.correntes()).Returns(ListaDeLeioloes);
            dao.Setup(d => d.atualiza(It.IsAny<Leilao>())).Throws(new Exception());
            var carteiro = new Mock<ICarteiro>();
            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            carteiro.Verify(c => c.envia(It.IsAny<Leilao>()), Times.Never);
        }
    }
}
