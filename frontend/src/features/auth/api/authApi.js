

const API_BASE = import.meta.env.VITE_API_BASE || "";

const TOKEN_KEY = "auth_token";
const USER_KEY = "auth_user";

function buildUrl(path) {
    if (!path.startsWith("/")) path = "/" + path;
    return `${API_BASE}${path}`;
}
ß
async function readProblemDetails(res) {
    
    const ct = res.headers.get("content-type") || "";
    try {
        if (ct.includes("application/json") || ct.includes("problem+json")) {
            return await res.json();
        }
        const text = await res.text();
        return text ? { detail: text } : null;
    } catch {
        return null;
    }
}

function toErrorMessage(payload, fallback) {
    if (!payload) return fallback;
    if (typeof payload === "string") return payload;

 
    if (payload.detail) return payload.detail;
    if (payload.title) return payload.title;


    if (payload.errors && typeof payload.errors === "object") {
        const firstKey = Object.keys(payload.errors)[0];
        const firstArr = payload.errors[firstKey];
        if (Array.isArray(firstArr) && firstArr.length > 0) return firstArr[0];
    }

   
    if (payload.message) return payload.message;

    return fallback;
}

function saveSession(token, user) {
    if (token) localStorage.setItem(TOKEN_KEY, token);
    if (user) localStorage.setItem(USER_KEY, JSON.stringify(user));
}

function clearSession() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
}

export function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

export function getStoredUser() {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
        return JSON.parse(raw);
    } catch {
        return null;
    }
}

export function getAuthHeader() {
    const token = getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
}

async function apiFetch(path, { method = "GET", body, auth = false } = {}) {
    const headers = {
        "Content-Type": "application/json",
        ...(auth ? getAuthHeader() : {}),
    };

    const res = await fetch(buildUrl(path), {
        method,
        headers,
        body: body ? JSON.stringify(body) : undefined,
    });

    if (!res.ok) {
        const payload = await readProblemDetails(res);
        const msg = toErrorMessage(payload, `Request failed (${res.status})`);
        const err = new Error(msg);
        err.status = res.status;
        err.payload = payload;
        throw err;
    }

  
    if (res.status === 204) return null;

    const ct = res.headers.get("content-type") || "";
    if (ct.includes("application/json")) return res.json();
    return res.text();
}

/**
 * Verwacht backend endpoints (pas aan als jouw routes anders zijn):
 * POST   /api/auth/register   { username, email, password }
 * POST   /api/auth/login      { usernameOrEmail, password }  (of username/email)
 * GET    /api/auth/me         (Bearer token)
 */
export async function register({ username, email, password }) {
    return apiFetch("/api/auth/register", {
        method: "POST",
        body: { username, email, password },
    });
}

export async function login({ usernameOrEmail, password }) {
    const data = await apiFetch("/api/auth/login", {
        method: "POST",
        body: { usernameOrEmail, password },
    });


    const token =
        (data && (data.token || data.accessToken || data.jwt)) ||
        (typeof data === "string" ? data : null);

    const user = data && typeof data === "object" ? data.user || data.gebruiker || null : null;

    if (token) saveSession(token, user);
    return { token, user };
}

export async function me() {
   
    try {
        const user = await apiFetch("/api/auth/me", { auth: true });
       
        
        if (user) saveSession(getToken(), user);
        return user;
    } catch {
        return getStoredUser();
    }
}

export function logout() {
    clearSession();
}
