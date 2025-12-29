const API_BASE = import.meta.env.VITE_API_BASE;

async function requestJson(path, options) {
    const res = await fetch(`${API_BASE}${path}`, options);
    const text = await res.text();

    let data = null;
    if (text) {
        try {
            data = JSON.parse(text);
        } catch {
            data = text;
        }
    }

    if (!res.ok) {
        const msg =
            typeof data === "object" && data && typeof data.message === "string"
                ? data.message
                : typeof data === "string"
                    ? data
                    : `HTTP ${res.status}`;
        throw new Error(msg);
    }

    return data;
}

function normalizeAuthResponse(data) {
    if (!data || typeof data !== "object") return { token: "", role: "" };
    const token = typeof data.token === "string" ? data.token : "";
    const role = typeof data.role === "string" ? data.role : "";
    return { token, role };
}

export async function login(email, password) {
    const data = await requestJson("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    const { token, role } = normalizeAuthResponse(data);
    if (!token) throw new Error("Ongeldige login");
    return { token, role };
}

export async function register({ email, password, voornaam, achternaam, rol }) {
    const data = await requestJson("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, voornaam, achternaam, rol }),
    });

    const { token, role } = normalizeAuthResponse(data);
    if (!token) throw new Error("Registratie mislukt");
    return { token, role };
}
