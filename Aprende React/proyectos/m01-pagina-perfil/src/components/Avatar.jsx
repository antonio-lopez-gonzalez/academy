// Avatar reutilizable con tamaño configurable y valor por defecto
export default function Avatar({ src, alt, size = 48 }) {
  return (
    <img
      className="avatar"
      src={src}
      alt={alt}
      // Estilo inline calculado a partir de la prop (objeto JS, camelCase)
      style={{ width: size, height: size }}
    />
  );
}
