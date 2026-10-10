import type { Post, Comment, User } from "./types";

const BASE_URL = "https://jsonplaceholder.typicode.com";

export async function getPosts(): Promise<Post[]> {
  const res = await fetch(`${BASE_URL}/posts`);
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return (await res.json()) as Post[];
}

export async function getPost(id: number): Promise<Post> {
  const res = await fetch(`${BASE_URL}/posts/${id}`);
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return (await res.json()) as Post;
}

export async function getComments(postId: number): Promise<Comment[]> {
  const res = await fetch(`${BASE_URL}/posts/${postId}/comments`);
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return (await res.json()) as Comment[];
}

export async function getUser(id: number): Promise<User> {
  const res = await fetch(`${BASE_URL}/users/${id}`);
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return (await res.json()) as User;
}