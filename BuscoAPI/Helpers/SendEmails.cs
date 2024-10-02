using BuscoAPI.DTOS;
using BuscoAPI.Services;

namespace BuscoAPI.Helpers
{
    public class SendEmails
    {
        public static MailRequest BuildEmailVerificationCode(int code, String type, String username)
        {
            string title = "";
            string message = "";
            string bottomMessage = "";

            if(type.Equals("recover-password", StringComparison.OrdinalIgnoreCase))
            {
                title = "Restablecer contraseña";
                message = "Has solicitado restablecer tu contraseña en Busco App.";
                bottomMessage = "Si tú no has solicitado restablecer tu contraseña, por favor, ignora este mensaje.";
            }
            else if(type.Equals("register", StringComparison.OrdinalIgnoreCase))
            {
                title = "Gracias por registrarte en Busco App!";
                message = "Nos alegra que decidas unirte a nuestra comunidad.";
                bottomMessage = "Si tú no has solicitado unirte a nuestra comunidad, por favor, ignora este mensaje.";
            }

            string codeString = code.ToString();
            string numbersSection = "<div class=\"numbers\">";
            foreach (char digit in codeString)
            {
                numbersSection += "\r\n        <div class=\"number\">\r\n          <span>" + digit + "</span>\r\n        </div>";
            }
            numbersSection += "\r\n      </div>";

            // HTML del correo electrónico
            var html = @"<head>
  <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
<link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
<link href=""https://fonts.googleapis.com/css2?family=Rubik:ital,wght@0,300..900;1,300..900&display=swap"" rel=""stylesheet"">
<style>
.body{
  background: #FF5722;
  font-family: ""Rubik"", sans-serif;
  display:flex;
  flex-direction:column;
  justify-content:center;
  align-items:center;
  padding:10px;
}

h1{
  font-weight: 700;  
  color: #4D4D4D;
  font-size:1.4em;
}

.section{
  width:50%;
  border-radius:10px;
  background: #D9D9D9;
}

.header{
  display:flex;
  align-items:center;
  background: rgb(256,256,256);
  border-radius:10px 10px 0px 0px;
  height:60px;
  padding:10px;
  margin:0;
}

.logo img{
  width:76px;
  height:76px;
  margin:0;
  margin-right:10px;
}

.main{
  padding-left:20px;
  padding-right:20px;
}

.numbers{
  display:flex;
  justify-content: space-around; 
  width:70%;
  margin: 28px auto;
}

.number{
  display:flex;
  align-items:center;
  justify-content:center;
  width:60px;
  height: 80px;
  background:#FF5722;
  border-radius:10px;
  color:white;
  font-size:36px;
  margin-right:5px;
}

.main-text p:first-child {
    margin-bottom: -14px; /* Puedes ajustar este valor según tus preferencias */
  }

.last-message{
  font-size:0.8em;
  color:white;
}
</style>
</head>

<body>
  <div class=""body"">
  <div class=""section"">
    <div class=""header"">
      <div class=""logo"">
        <img src=""https://i.pinimg.com/736x/1f/7b/2d/1f7b2d71ad83f993902a4390a21fdc7e.jpg"">
      </div>
      <h1>"+title +@"</h1>
    </div>
    <div class=""main"">
      <div class=""main-text"">
        <p>Hola "+username+@"!</p>
        <p>"+message+@"</p>
        <p>Aquí está tu código de verificación:</p>
      </div>
" + numbersSection + @"
    </div>
  </div>
  
  <p class=""last-message"">"+bottomMessage+@"</p>
    </div>
</body>";

            
            var emailReq = new MailRequest()
            {
                Subject = "Verification code for Busco app",
                Body = html
            };

            return emailReq;
        }

    }
}
