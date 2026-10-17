export default function PasswordStrength({ password }) {
  if (!password) return null;

  let strength = 0;
  if (password.length >= 8) strength++;
  if (/[A-Z]/.test(password)) strength++;
  if (/[0-9]/.test(password)) strength++;
  if (/[^A-Za-z0-9]/.test(password)) strength++;

  const labels = ["Muy débil", "Débil", "Media", "Fuerte", "Muy fuerte"];
  const colors = ["#ff4444", "#ff8800", "#ffcc00", "#88cc00", "#44bb44"];

  return (
    <div className="password-strength">
      <div className="strength-bar">
        <div
          className="strength-fill"
          style={{ width: `${(strength / 4) * 100}%`, backgroundColor: colors[strength] }}
        />
      </div>
      <span style={{ color: colors[strength] }}>{labels[strength]}</span>
    </div>
  );
}
