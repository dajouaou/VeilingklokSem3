import { useContext } from "react";
import Navbar from "../shared/components/Navbar";
import Footer from "../shared/components/Footer";
import { AuthContext } from "../features/auth/AuthContext";
import useLiveVeiling from "../features/veilingmeesterDashboard/hooks/useLiveVeiling";

export default function ActueelBod() {
    const { token, role } = useContext(AuthContext);
    const veilingId = 1;

    const { lot, loading } = useLiveVeiling(token, veilingId);

    const currentLot = lot;
    const currentPrice = lot?.huidigePrijs;
    const isStopped = !lot;

    async function neemDezePrijs() {
        if (!currentLot || !currentPrice || !token) return;

        await fetch(`https://localhost:56418/api/bod/${veilingId}`, {
            method: "POST",
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                veilingProductId: currentLot.veilingProductId,
                prijs: currentPrice
            })
        });
    }

    if (loading) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Veiling laden…</div>
                <Footer />
            </>
        );
    }

    if (!currentLot) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Geen actieve veiling</div>
                <Footer />
            </>
        );
    }

    return (
        <>
            <Navbar />

            <div className="container py-5">
                <h2 className="fw-bold mb-4">Actueel bod</h2>

                <div className="row">
                    <div className="col-md-6">
                        <img
                            src={currentLot.fotoUrl}
                            alt={currentLot.soort}
                            className="img-fluid rounded"
                        />
                    </div>

                    <div className="col-md-6">
                        <span className="badge bg-success">
                            {isStopped ? "Afgesloten" : "Actief"}
                        </span>

                        <h4 className="mt-2">{currentLot.soort}</h4>
                        <small>Hoeveelheid: {currentLot.hoeveelheid}</small>

                        <h5 className="mt-4">
                            Huidige prijs: € {currentPrice?.toFixed(2)}
                        </h5>

                        {role === "Koper" && !isStopped && (
                            <button
                                className="btn btn-dark mt-3"
                                onClick={neemDezePrijs}
                            >
                                Neem deze prijs
                            </button>
                        )}
                    </div>
                </div>
            </div>

            <Footer />
        </>
    );
}
