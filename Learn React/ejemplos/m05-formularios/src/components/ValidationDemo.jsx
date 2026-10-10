import { useState } from "react";

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function ValidationDemo() {
  const [email, setEmail] = useState("");
  const [touched, setTouched] = useState(false);

  const error = email === ""
    ? "El email es obligatorio"
    : !EMAIL_REGEX.test(email)
    ? "Email inválido"
    : "";

  const isValid = error === "";

  function handleSubmit(e) {
    e.preventDefault();
    if (isValid) {
      alert(`Enviado a: ${email}`);
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        onBlur={() => setTouched(true)}
        placeholder="tu@email.com"
      />
      {touched && error && <span className="error">{error}</span>}
      <button type="submit" disabled={!isValid}>Enviar</button>
    </form>
  );
}
