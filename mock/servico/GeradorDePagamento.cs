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
        private IRelogio relogio;

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao, Avaliador avaliador, IPagamentoDao pagamentoDao, IRelogio relogio)
        {
            this.leilaoDao = leilaoDao;
            this.avaliador = avaliador;
            this.pagamentoDao = pagamentoDao;
            this.relogio = relogio;
        }

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao,Avaliador avaliador,IPagamentoDao pagamentoDao) : this(leilaoDao,avaliador,pagamentoDao,new RelogioDoSistema())
        {

        }

        public void gera()
        {
            var encerrados = leilaoDao.encerrados();
            foreach (var l in encerrados)
            {
                avaliador.avalia(l);
                var pagamento = new Pagamento(avaliador.maiorValor, ProximoDiaUtil());
                pagamentoDao.Salvar(pagamento);
            }
        }

        private DateTime ProximoDiaUtil()
        {
            var data = relogio.hoje();
            var diaDaSemana = data.DayOfWeek;

            if (diaDaSemana == DayOfWeek.Saturday)
                data = data.AddDays(2);
            else if (diaDaSemana == DayOfWeek.Sunday)
                data = data.AddDays(1);

            return data;
        }
    }
}
