class FirebaseAuthHelper {
    constructor() {
        this.finished = false;
        this.token = "";
        this.created = false;
    }
    CreateUser(email, pass) {
        let parent = this;
        firebase.auth()
            .createUserWithEmailAndPassword(email, pass)
            .then((userCredential) => {
                parent.finished = true;
                parent.created = true;
            })
            .catch((error) => {
                console.log(error);
                parent.finished = true;
            });
    }
    LoginCredentials(email, pass) {
        let parent = this;
        firebase.auth().signInWithEmailAndPassword(email, pass)
            .then(() => {
                firebase
                    .auth()
                    .currentUser
                    .getIdToken(true)
                    .then((jwtToken) => {
                        parent.finished = true;
                        parent.token = jwtToken;
                    }).catch((error) => {
                        parent.finished = true;
                    });
                firebase.auth().signOut();
            })
            .catch((error) => {
                console.log(error);
                parent.finished = true;
            });
    }
    Login(provider) {
        let parent = this;
        firebase
            .auth()
            .signInWithPopup(provider)
            .then(() => {
                firebase
                    .auth()
                    .currentUser
                    .getIdToken(true)
                    .then((jwtToken) => {
                        parent.finished = true;
                        parent.token = jwtToken;
                    }).catch((error) => {
                        parent.finished = true;
                    });
                firebase.auth().signOut();
            })
            .catch((error) => {
                parent.finished = true;
            });
    }
}
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
var firebaseConfig = {
    apiKey: "API",
    authDomain: "Domain",
    projectId: "ProjectID",
    storageBucket: "Storage",
    messagingSenderId: "SenderId",
    appId: "AppId",
    measurementId: "MeasurementId"
};
// Initialize Firebase
firebase.initializeApp(firebaseConfig);
firebase.auth().setPersistence(firebase.auth.Auth.Persistence.NONE);

/*
 Metodo de login baseado em servidores de terceiros
 Mais informações em: https://firebase.google.com/docs/auth/web/google-signin
*/
export async function LoginGoogle() {

    let loginModel = new FirebaseAuthHelper();

    let providerGoogle = new firebase.auth.GoogleAuthProvider();

    /*
     Está linha está aqui para caso você faça o
     Logoff da conta, quando entrar novamente
     ele solicite para que você informe qual
     conta deseja acessar, sem esta linha
     ele vai automaticamente acessar a ultima
     conta google valida
    */
    providerGoogle.setCustomParameters({
        prompt: 'select_account'
    });

    /*
     É necessario solicitar ao menos um perfil 
     quando se faz autenticação com servidores,
     este é um perfil basico onde tem informações
     como nome e email da pessoa
     Mais informações em: https://firebase.google.com/docs/auth/web/google-signin
    */
    providerGoogle.addScope("profile");

    loginModel.Login(providerGoogle);
    while (!loginModel.finished) {
        await sleep(1000);
    }
    return loginModel.token;
}
/*
 Metodo tradicional de login e senha
 Mais informações em: https://firebase.google.com/docs/auth/web/start
*/
export async function LoginCredentials(user, pass) {
    let loginModel = new FirebaseAuthHelper();

    loginModel.LoginCredentials(user, pass);
    while (!loginModel.finished) {
        await sleep(1000);
    }
    return loginModel.token;
}
/*
 Metodo tradicional de criação de usuario
 Mais informações em: https://firebase.google.com/docs/auth/web/start
*/
export async function CreateUser(user, pass) {
    let loginModel = new FirebaseAuthHelper();
    loginModel.CreateUser(user, pass);
    while (!loginModel.finished) {
        await sleep(1000);
    }
    return loginModel.created;
}
