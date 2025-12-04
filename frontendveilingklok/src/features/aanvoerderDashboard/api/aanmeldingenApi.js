export async function getAanmeldingen(token) {
    const res = await fetch("https://localhost:56418/api/aanmeldingen", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`,  // ? Token meesturen
            "Content-Type": "application/json"
        }
    });

    if (!res.ok) {
        throw new Error("Kan aanmeldingen niet ophalen.");
    }

    return await res.json();
}
