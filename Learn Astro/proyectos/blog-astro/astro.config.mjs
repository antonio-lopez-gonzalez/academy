// @ts-check
import { defineConfig } from "astro/config";
import mdx from "@astrojs/mdx";
import sitemap from "@astrojs/sitemap";

// `site` es obligatorio para sitemap y URLs canónicas.
export default defineConfig({
  site: "https://blog.example.com",
  integrations: [mdx(), sitemap()],
});
