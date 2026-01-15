export default function ReviewSection() {
    return (
        <section id="reviews" className="py-5">
            <div className="container">
                <h2 className="fw-bold mb-4 text-center">Wat onze bieders zeggen</h2>
                <div className="row g-4">

                    {[
                        {
                            naam: "Doa",
                            rol: "Bloemenliefhebber",
                            img: "Images/icon.jpg",
                            text: "Geweldige service en mooie kwaliteit bloemen. Elke veiling is spannend en overzichtelijk!",
                            stars: 5
                        },
                        {
                            naam: "Shriya",
                            rol: "Tuinier",
                            img: "Images/icon.jpg",
                            text: "De digitale veiling is super makkelijk te gebruiken en de producten zijn altijd topkwaliteit.",
                            stars: 4
                        },
                        {
                            naam: "Sofia",
                            rol: "Plantenhandelaar",
                            img: "Ïmages/icon.jpg",
                            text: "Ik kan de veiling elke week volgen en vind altijd de bloemen die ik nodig heb. Top ervaring!",
                            stars: 5
                        }
                    ].map((r, index) => (
                        <div className="col-md-4" key={index}>
                            <div className="card card-review h-100 p-3">

                                <div className="d-flex align-items-center mb-3">
                                    <img src={r.img} alt={r.naam} className="review-img me-3" />
                                    <div>
                                        <h6 className="mb-0 fw-semibold">{r.naam}</h6>
                                        <small className="text-muted">{r.rol}</small>
                                    </div>
                                </div>

                                <p className="review-text">{r.text}</p>

                                <div className="review-stars">
                                    {"★".repeat(r.stars)}{"☆".repeat(5 - r.stars)}
                                </div>
                            </div>
                        </div>
                    ))}

                </div>
            </div>
        </section>
    );
}
