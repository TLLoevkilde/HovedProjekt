import { UserManager, WebStorageStateStore } from 'https://cdn.skypack.dev/oidc-client-ts';

const apiUrl = "https://localhost:7126/api/note";
const clientId = "spa-client";
const redirectUri = "https://localhost:7296/callback";
const authority = "https://localhost:7143";

const settings = {
    authority: authority,
    client_id: clientId,
    redirect_uri: redirectUri,
    response_type: "code",
    scope: "openid profile email note_api",
    post_logout_redirect_uri: "https://localhost:7296/",
    userStore: new WebStorageStateStore({ store: window.localStorage }),
    automaticSilentRenew: true,
    silent_redirect_uri: redirectUri.replace("callback", "silent-renew")
};

const userManager = new UserManager(settings);

const loginBtn = document.getElementById("loginBtn");
const logoutBtn = document.getElementById("logoutBtn");
const notesSection = document.getElementById("notesSection");
const addNoteBtn = document.getElementById("addNoteBtn");
const noteInput = document.getElementById("noteInput");
const noteList = document.getElementById("noteList");

loginBtn.onclick = () => userManager.signinRedirect();
logoutBtn.onclick = () => userManager.signoutRedirect();
addNoteBtn.onclick = addNote;

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

async function loadNotes() {
    const user = await userManager.getUser();
    if (!user) return alert("Du skal logge ind");

    const res = await fetch(apiUrl, {
        headers: { Authorization: `Bearer ${user.access_token}` }
    });
    if (!res.ok) return alert("Fejl ved hentning af noter");

    const notes = await res.json();
    noteList.innerHTML = "";
    notes.forEach(note => {
        const li = document.createElement("li");
        li.innerText = note;
        noteList.appendChild(li);
    });
}

async function addNote() {
    const note = noteInput.value.trim();
    if (!note) return;

    const user = await userManager.getUser();
    if (!user) return alert("Du skal logge ind");

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
