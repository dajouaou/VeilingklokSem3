const API_BASE = "https://localhost:56418";

export async function fetchAanmeldingen({ token, veildatum }) {
    const params = new URLSearchParams();
    if (veildatum) params.set("veildatum", veildatum);

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen?${params.toString()}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    });

    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.message || "Kon aanmeldingen niet laden.");
    }

    return res.json();
}

export async function createAanmelding({ token, data }) {
    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify(data),
    });

    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.message || "Kon aanmelding niet opslaan.");
    }

    return res.json();
}

export async function fetchAanvoerderStats({ token, veildatum }) {
    const params = new URLSearchParams();
    if (veildatum) params.set("veildatum", veildatum);

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/statistieken?${params.toString()}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    });

    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.message || "Kon statistieken niet laden.");
    }

    return res.json();
}
