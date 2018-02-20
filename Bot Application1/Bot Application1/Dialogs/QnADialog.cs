using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class QnADialog : QnAMakerDialog
    {
        public QnADialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnASubscriptionKey"], ConfigurationManager.AppSettings["QnAKnowledgeBaseId"],"Não Encontrei sua resposta", 0.5)))
            {
            
            }
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers.First().Answer;

            Activity resposta = ((Activity)context.Activity).CreateReply();

            var dadosResposta = primeiraResposta.Split(';');

            if (dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var Titulo = dadosResposta[0];

            var descricao = dadosResposta[1];

            var url = dadosResposta[2];

            var imgUrl = dadosResposta[3];

            HeroCard card = new HeroCard
            {
                Title = Titulo,
                Subtitle = descricao,
            };

            card.Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.OpenUrl,"SaibaMais",value:url)
            };

            card.Images = new List<CardImage>
            {
               new CardImage(url = imgUrl)
            };

            resposta.Attachments.Add(card.ToAttachment());

            await context.PostAsync(resposta);
        }
    }
}