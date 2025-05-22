import { UserManager, WebStorageStateStore } from 'https://cdn.skypack.dev/oidc-client-ts';

const apiUrl = "https://localhost:7126/api/note";
const clientId = "spa-client";
const redirectUri = "https://localhost:7296/callback";
const authority = "https://localhost:7143";

const settings = {
    authority,
    client_id: clientId,
    redirect_uri: redirectUri,
    response_type: "code",
    scope: "openid note_api offline_access",
    post_logout_redirect_uri: "https://localhost:7296/",
    userStore: new WebStorageStateStore({ store: window.localStorage })
};

const userManager = new UserManager(settings);

// UI-elementer
const loginBtn = document.getElementById("loginBtn");
const logoutBtn = document.getElementById("logoutBtn");
const notesSection = document.getElementById("notesSection");
const addNoteBtn = document.getElementById("addNoteBtn");
const noteInput = document.getElementById("noteInput");
const noteList = document.getElementById("noteList");

// Bind knapper
loginBtn.onclick = () => userManager.signinRedirect();
logoutBtn.onclick = () => userManager.signoutRedirect();
addNoteBtn.onclick = addNote;

// Når siden loader
window.onload = async () => {
    if (window.location.pathname.endsWith("/callback")) {
        try {
            await userManager.signinRedirectCallback();
            window.history.replaceState({}, document.title, "/");
        } catch (e) {
            try {
                await userManager.signoutRedirectCallback();
                window.location.href = "/";
            } catch (logoutError) {
                console.error("Fejl ved logout callback:", logoutError);
            }
        }
    }
    await updateUI();
};

// Hent noter
async function loadNotes() {
    let user = await ensureValidUser();
    if (!user) return;

    const res = await fetch(apiUrl, {
        headers: { Authorization: `Bearer ${user.access_token}` }
    });
    if (!res.ok) return alert("Fejl ved hentning af noter");

    const notes = await res.json();
    noteList.innerHTML = "";
    notes.forEach(n => {
        const li = document.createElement("li");
        li.innerText = n;
        noteList.appendChild(li);
    });
}

// Tilføj note
async function addNote() {
    let user = await ensureValidUser();
    if (!user) return;

    const note = noteInput.value.trim();
    if (!note) return;

    const res = await fetch(apiUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.access_token}`
        },
        body: JSON.stringify(note)
    });
    if (!res.ok) return alert("Fejl ved at tilføje note");

    noteInput.value = "";
    loadNotes();
}

// Opdater UI baseret på loginstatus
async function updateUI() {
    const user = await userManager.getUser();
    if (user && !user.expired) {
        loginBtn.style.display = "none";
        logoutBtn.style.display = "inline";
        notesSection.style.display = "block";
        loadNotes();
    } else {
        loginBtn.style.display = "inline";
        logoutBtn.style.display = "none";
        notesSection.style.display = "none";
    }
}

// Sikr at vi har et gyldigt token (forny hvis udløbet)
async function ensureValidUser() {
    let user = await userManager.getUser();
    if (!user) {
        alert("Du skal logge ind");
        return null;
    }
    if (user.expired) {
        await renewToken();
        user = await userManager.getUser();
        if (!user || user.expired) {
            alert("Kunne ikke forny token – log ind igen");
            return null;
        }
    }
    return user;
}

// Forny access token med refresh token
async function renewToken() {
    const user = await userManager.getUser();
    if (!user || !user.refresh_token) {
        console.warn("Ingen refresh token tilgængelig");
        return;
    }

    const body = new URLSearchParams();
    body.append("grant_type", "refresh_token");
    body.append("client_id", clientId);
    body.append("refresh_token", user.refresh_token);

    const res = await fetch(`${authority}/connect/token`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: body.toString()
    });

    if (res.ok) {
        const newTokens = await res.json();
        const updatedUser = {
            ...user,
            access_token: newTokens.access_token,
            expires_at: Math.floor(Date.now() / 1000) + newTokens.expires_in,
            refresh_token: newTokens.refresh_token ?? user.refresh_token
        };
        await userManager.storeUser(updatedUser);
        console.log("Access token fornyet");
    } else {
        console.error("Kunne ikke forny token med refresh_token:", await res.text());
    }
}
