const API = import.meta.env.VITE_API_BASE;

export async function login(email, password) {
    const res = await fetch(`${API}/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (!res.ok) throw new Error("Login mislukt");

    return res.json();
}

const API_URL = "https://localhost:56418/api/Auth";


export async function loginApi({ email, password }) {
    const res = await fetch(`${API_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
        const err = await res.json();
        throw new Error(err?.message || "Login mislukt");
    }

    return res.json(); //  token 
}

export async function registerApi({ email, password, voornaam, achternaam, rol }) {
    const res = await fetch(`${API_URL}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, voornaam, achternaam, rol }),
    });

    if (!res.ok) {
        const err = await res.json();
        throw new Error(err?.message || "Registratie mislukt");
    }

    return res.json(); // token 
}