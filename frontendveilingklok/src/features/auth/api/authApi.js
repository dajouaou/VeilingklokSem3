const API_URL = "https://localhost:7043/api/Auth";

export async function loginApi(email, password) {
    const res = await fetch(`${API_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || "Login mislukt.");
    }

    return await res.json();
}

export async function registerApi(data) {
    const res = await fetch(`${API_URL}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || "Registratie mislukt.");
    }

    return await res.json();
}
