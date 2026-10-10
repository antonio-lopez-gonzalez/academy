import { useState } from "react";
import PasswordStrength from "./PasswordStrength";
import FormSummary from "./FormSummary";

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PASSWORD_REGEX = /^(?=.*[A-Za-z])(?=.*\d).{8,}$/;

export default function RegistrationForm() {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [birthDate, setBirthDate] = useState("");
  const [country, setCountry] = useState("");
  const [plan, setPlan] = useState("");
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [touched, setTouched] = useState({});
  const [submitted, setSubmitted] = useState(false);

  function isOver18(dateStr) {
    if (!dateStr) return false;
    const birth = new Date(dateStr);
    const today = new Date();
    const age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    return age > 18 || (age === 18 && monthDiff >= 0 && today.getDate() >= birth.getDate());
  }

  const errors = {
    name: name.trim().length < 3 ? "Mínimo 3 caracteres" : "",
    email: !EMAIL_REGEX.test(email) ? "Email inválido" : "",
    password: !PASSWORD_REGEX.test(password) ? "Mínimo 8 caracteres, con letras y números" : "",
    confirmPassword: password !== confirmPassword ? "No coincide" : "",
    birthDate: !isOver18(birthDate) ? "Debes ser mayor de 18 años" : "",
    country: country === "" ? "Selecciona un país" : "",
    plan: plan === "" ? "Selecciona un plan" : "",
    acceptTerms: !acceptTerms ? "Debes aceptar los términos" : "",
  };

  const hasErrors = Object.values(errors).some((err) => err !== "");

  function handleSubmit(e) {
    e.preventDefault();
    setTouched({
      name: true, email: true, password: true, confirmPassword: true,
      birthDate: true, country: true, plan: true, acceptTerms: true,
    });
    if (!hasErrors) {
      setSubmitted(true);
    }
  }

  if (submitted) {
    return (
      <div className="success-message">
        <h2>¡Registro completado!</h2>
        <p>Bienvenido, {name}. Tu cuenta ha sido creada con éxito.</p>
      </div>
    );
  }

  return (
    <div className="registration-container">
      <form onSubmit={handleSubmit} className="registration-form">
        <div className="field">
          <label>Nombre</label>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, name: true }))}
            placeholder="Tu nombre"
          />
          {touched.name && errors.name && <span className="error">{errors.name}</span>}
        </div>

        <div className="field">
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, email: true }))}
            placeholder="tu@email.com"
          />
          {touched.email && errors.email && <span className="error">{errors.email}</span>}
        </div>

        <div className="field">
          <label>Contraseña</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, password: true }))}
            placeholder="Mínimo 8 caracteres"
          />
          {touched.password && errors.password && <span className="error">{errors.password}</span>}
          <PasswordStrength password={password} />
        </div>

        <div className="field">
          <label>Confirmar contraseña</label>
          <input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, confirmPassword: true }))}
            placeholder="Repite la contraseña"
          />
          {touched.confirmPassword && errors.confirmPassword && (
            <span className="error">{errors.confirmPassword}</span>
          )}
        </div>

        <div className="field">
          <label>Fecha de nacimiento</label>
          <input
            type="date"
            value={birthDate}
            onChange={(e) => setBirthDate(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, birthDate: true }))}
          />
          {touched.birthDate && errors.birthDate && <span className="error">{errors.birthDate}</span>}
        </div>

        <div className="field">
          <label>País</label>
          <select
            value={country}
            onChange={(e) => setCountry(e.target.value)}
            onBlur={() => setTouched((p) => ({ ...p, country: true }))}
          >
            <option value="">Selecciona un país</option>
            <option value="es">España</option>
            <option value="mx">México</option>
            <option value="ar">Argentina</option>
            <option value="co">Colombia</option>
            <option value="cl">Chile</option>
            <option value="pe">Perú</option>
          </select>
          {touched.country && errors.country && <span className="error">{errors.country}</span>}
        </div>

        <fieldset className="field">
          <legend>Plan</legend>
          {["free", "pro", "enterprise"].map((p) => (
            <label key={p} className="radio-label">
              <input
                type="radio"
                name="plan"
                value={p}
                checked={plan === p}
                onChange={(e) => setPlan(e.target.value)}
              />
              {p.charAt(0).toUpperCase() + p.slice(1)}
            </label>
          ))}
          {touched.plan && errors.plan && <span className="error">{errors.plan}</span>}
        </fieldset>

        <div className="field">
          <label className="checkbox-label">
            <input
              type="checkbox"
              checked={acceptTerms}
              onChange={(e) => setAcceptTerms(e.target.checked)}
              onBlur={() => setTouched((p) => ({ ...p, acceptTerms: true }))}
            />
            Acepto los términos y condiciones
          </label>
          {touched.acceptTerms && errors.acceptTerms && (
            <span className="error">{errors.acceptTerms}</span>
          )}
        </div>

        <button type="submit" disabled={hasErrors}>Registrarse</button>
      </form>

      <FormSummary data={{ name, email, country, plan, acceptTerms }} />
    </div>
  );
}
