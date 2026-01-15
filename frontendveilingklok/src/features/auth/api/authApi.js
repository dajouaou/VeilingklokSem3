import { API_BASE_URL } from "../../../config/apiBaseUrl";

export async function loginApi({ email, password }) {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Email: email, Password: password }),
    });

    if (!res.ok) {
        const err = await res.json().catch(() => null);
        throw new Error(err?.message || "Login mislukt");
    }

    return res.json(); // { token, role }
}

export async function registerApi({ email, password, voornaam, achternaam, rol }) {
    const res = await fetch(`${API_BASE_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Email: email,
            Password: password,
            Voornaam: voornaam,
            Achternaam: achternaam,
            Rol: Number(rol),
        }),
    });

    if (!res.ok) {
        const err = await res.json().catch(() => null);
        throw new Error(err?.message || "Registratie mislukt");
    }

    return res.json(); // { token, role } of { token }
}
