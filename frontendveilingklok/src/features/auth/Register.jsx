import { useState, useContext } from "react";
import { registerApi } from "./api/authApi";
import { AuthContext } from "./AuthContext";
import { useNavigate } from "react-router-dom";

export default function Register() {
    const [form, setForm] = useState({
        email: "",
        voornaam: "",
        achternaam: "",
        password: "",
        rol: 0, // 0 = Koper, 1 = Aanvoerder
    });

    const [error, setError] = useState("");
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");

        try {
            const { token } = await registerApi(form);

            login(token, form.rol === 0 ? "Koper" : "Aanvoerder");

            navigate(form.rol === 0 ? "/koper" : "/aanvoerder");
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
                                        onChange={(e) => setForm({ ...form, voornaam: e.target.value })}
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="Achternaam"
                                        onChange={(e) => setForm({ ...form, achternaam: e.target.value })}
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="email"
                                        className="form-control"
                                        placeholder="E-mailadres"
                                        onChange={(e) => setForm({ ...form, email: e.target.value })}
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        placeholder="Wachtwoord"
                                        onChange={(e) => setForm({ ...form, password: e.target.value })}
                                        required
                                    />
                                </div>

                                {/* Rol keuze */}
                                <div className="mb-3">
                                    <select
                                        className="form-control"
                                        onChange={(e) => setForm({ ...form, rol: Number(e.target.value) })}
                                    >
                                        <option value="0">Koper</option>
                                        <option value="1">Aanvoerder</option>
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
