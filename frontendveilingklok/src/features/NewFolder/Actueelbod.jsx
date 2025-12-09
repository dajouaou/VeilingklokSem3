import { useEffect, useState } from "react";
import Navbar from "../shared/components/Navbar";
import Footer from "../shared/components/Footer";

const API = "https://localhost:56418";

export default function ActueelBod() {

    const [product, setProduct] = useState(null);
    const [prijs, setPrijs] = useState(null);

    async function loadData() {
        try {
            const res = await fetch(`${API}/api/koper/actueel`);
            const data = await res.json();

            console.log("API result:", data);

            if (!data || !data.huidigProduct) return;

            setProduct(data.huidigProduct);
            setPrijs(data.huidigProduct.huidigePrijs);

        } catch (e) {
            console.log("fout:", e);
        }
    }

    useEffect(() => {
        loadData();
        const timer = setInterval(loadData, 3000);
        return () => clearInterval(timer);
    }, []);

    async function neemDezePrijs() {
        await fetch(`${API}/api/koper/biedingen`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ prijs })
        });
    }

    if (!product) return <><Navbar /><div className="container py-5">Laden...</div><Footer /></>;

    return (
        <>
            <Navbar />

            <section className="py-5 bg-light">
                <div className="container">
                    <h2 className="mb-4 fw-bold">Actueel bod</h2>

                    <div className="card shadow-sm border-0 overflow-hidden">

                        <div className="row g-0">

                            {/* LEFT IMAGE */}
                            <div className="col-md-6">
                                <img
                                    src={product.fotoUrl}
                                    alt=""
                                    className="img-fluid h-100 object-fit-cover"
                                />
                            </div>

                            {/* RIGHT PART */}
                            <div className="col-md-6 p-4">

                                <span className="badge bg-success mb-2">Actief</span>
                                <h4>{product.soort}</h4>

                                <p className="text-muted mb-2">
                                    Hoeveelheid: {product.hoeveelheid}
                                </p>

                                <p className="fw-bold">
                                    Laatste prijs:
                                    <span className="ms-2 text-primary">
                                        € {prijs?.toFixed(2)}
                                    </span>
                                </p>

                                {/* Progress bar */}
                                <div className="progress mt-2 mb-3" style={{ height: "6px" }}>
                                    <div
                                        className="progress-bar bg-success"
                                        role="progressbar"
                                        style={{ width: "60%" }}
                                    ></div>
                                </div>

                                {/* Button */}
                                <button className="btn btn-dark px-4" onClick={neemDezePrijs}>
                                    Neem deze prijs
                                </button>

                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <Footer />
        </>
    );

}
