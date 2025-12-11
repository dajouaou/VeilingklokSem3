// src/features/veiling/VeilingPage.jsx
import useVeilingRealtime from "./hooks/useVeilingRealtime";

const VEILING_ID = 1;

export default function VeilingPage() {
    const {
        loading,
        error,
        currentProduct,
        queue,
        bids,
        timeLeftMs,
        currentPrice,
        placeBid,
        isLoggedIn,
        highestBidderId,
    } = useVeilingRealtime(VEILING_ID);

    if (loading) {
        return <div>Veiling wordt geladen…</div>;
    }

    if (error) {
        return <div> {error}</div>;
    }

    if (!currentProduct) {
        return <div>Geen actief product in deze veiling.</div>;
    }

    return (
        <div style={{ padding: "1rem" }}>
            <h1>Publieke veiling #{VEILING_ID}</h1>

            <section style={{ marginBottom: "1rem" }}>
                <h2>{currentProduct.product?.naam ?? "Onbekend product"}</h2>
                <p>Status: {currentProduct.status}</p>
                <p>Hoeveelheid: {currentProduct.hoeveelheid}</p>
                <p>Startprijs: € {(currentProduct.startPrijs ?? 0).toFixed(2)}</p>
                <p>Huidige prijs: € {currentPrice.toFixed(2)}</p>
                <p>Resterende tijd: {Math.max(0, Math.floor(timeLeftMs / 1000))}s</p>
                {highestBidderId && (
                    <p>Huidige hoogste bieder: #{highestBidderId}</p>
                )}
            </section>

            <section style={{ marginBottom: "1rem" }}>
                <button onClick={placeBid} disabled={!isLoggedIn}>
                    {isLoggedIn ? "Plaats bod" : "Log in om te kunnen bieden"}
                </button>
            </section>

            <section style={{ marginBottom: "1rem" }}>
                <h3>Wachtende producten (queue)</h3>
                {queue.length === 0 && <p>Geen producten in de wachtrij.</p>}
                {queue.map((item) => (
                    <div key={item.id}>
                        #{item.volgorde} — {item.product?.naam ?? "Onbekend product"}
                    </div>
                ))}
            </section>

            <section>
                <h3>Laatste biedingen</h3>
                {bids.length === 0 && <p>Nog geen biedingen.</p>}
                {bids.map((bid) => (
                    <div key={bid.id}>
                        € {bid.amount} door {bid.koperNaam ?? `koper #${bid.koperId}`}
                    </div>
                ))}
            </section>
        </div>
    );
}
