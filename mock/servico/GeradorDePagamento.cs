using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class GeradorDePagamento
    {
        private LeilaoDaoFalso leilaoDao;
        private Avaliador avaliador;
        private IPagamentoDao pagamentoDao;

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao,Avaliador avaliador,IPagamentoDao pagamentoDao)
        {
            this.leilaoDao = leilaoDao;
            this.avaliador = avaliador;
            this.pagamentoDao = pagamentoDao;
        }

        public void gera()
        {
            var encerrados = leilaoDao.encerrados();
            foreach (var l in encerrados)
            {
                avaliador.avalia(l);
                var pagamento = new Pagamento(avaliador.maiorValor, DateTime.Today);
                pagamentoDao.Salvar(pagamento);
            }
        }
    }
}
