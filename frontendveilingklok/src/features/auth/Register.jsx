import { useState } from "react";
import { registerApi } from "../auth/api/authApi";
import { useNavigate } from "react-router-dom";

export default function Register() {
    const navigate = useNavigate();

    const [form, setForm] = useState({
        email: "",
        voornaam: "",
        achternaam: "",
        password: "",
        rol: "",
    });

    const [error, setError] = useState("");

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");

        try {
            await registerApi(form);

            alert("Succesvol geregistreerd! Je kunt nu inloggen.");
            navigate("/login");

        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="bg-light">
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-6 col-lg-4">
                        <div className="card shadow-sm border-0 rounded-4 p-4">

                            <h2 className="fw-bold mb-3 text-success text-center">
                                Registreren
                            </h2>

                            {error && <div className="alert alert-danger">{error}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="Voornaam"
                                        value={form.voornaam}
                                        onChange={(e) =>
                                            setForm({ ...form, voornaam: e.target.value })
                                        }
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="Achternaam"
                                        value={form.achternaam}
                                        onChange={(e) =>
                                            setForm({ ...form, achternaam: e.target.value })
                                        }
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="email"
                                        className="form-control"
                                        placeholder="E-mailadres"
                                        value={form.email}
                                        onChange={(e) =>
                                            setForm({ ...form, email: e.target.value })
                                        }
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        placeholder="Wachtwoord"
                                        value={form.password}
                                        onChange={(e) =>
                                            setForm({ ...form, password: e.target.value })
                                        }
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <select
                                        className="form-control"
                                        value={form.rol}
                                        onChange={(e) =>
                                            setForm({ ...form, rol: e.target.value })
                                        }
                                        required
                                    >
                                        <option value="">Kies een rol…</option>
                                        <option value="Koper">Koper</option>
                                        <option value="Aanvoerder">Aanvoerder</option>
                                        <option value="Veilingmeester">Veilingmeester</option>
                                    </select>
                                </div>

                                <button className="btn btn-success w-100 rounded-pill">
                                    Account aanmaken
                                </button>
                            </form>

                            <p className="text-center mt-4">
                                Al een account?{" "}
                                <a href="/login" className="text-success fw-semibold">
                                    Log in
                                </a>
                            </p>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
