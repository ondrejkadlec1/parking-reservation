const msalConfig = {
    auth: {
        clientId: "41691fcc-4f37-43c2-852d-171ed346d12b",
        authority: "https://login.microsoftonline.com/7dfadd65-f94a-4e0a-a0cc-3d5b47a76ad5",
        redirectUri: window.location.href
    },
    cache: {
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: false
    }
};

const msalInstance = new msal.PublicClientApplication(msalConfig);

const loginRequest = {
    scopes: ["api://6f71637b-2eef-4148-855f-2897dfabfd84/App.ReadWrite"]
};

let accessToken = null;

async function signIn() {
    try {
        const loginResponse = await msalInstance.loginPopup(loginRequest);
        const accounts = msalInstance.getAllAccounts();
        if (accounts.length > 0) {
            msalInstance.setActiveAccount(accounts[0]);
        }

        const tokenResponse = await msalInstance.acquireTokenSilent(loginRequest);

        accessToken = tokenResponse.accessToken;
        $.ajaxSetup({
            headers: { "Authorization": `Bearer ${accessToken}` }
        });
    } catch (error) {
        console.error("Token acquisition failed", error);
    }
    
}