const API_URL = "https://localhost:5001/api/Auth";
// Zorg dat dit klopt met jouw backend URL

export async function loginApi(email, password) {
    const res = await fetch(`${API_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (!res.ok) throw new Error("Login mislukt.");
    return await res.json(); // { token: "...", role: "..." }
}

export async function registerApi(data) {
    const res = await fetch(`${API_URL}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error("Registratie mislukt: " + text);
    }

    return await res.json(); // { token: "..." }
}
