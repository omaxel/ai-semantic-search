import os

import psycopg2
from PIL import Image
from fastapi import FastAPI
from sentence_transformers import SentenceTransformer

model = SentenceTransformer('clip-ViT-B-32')

app = FastAPI()
@app.get("/embed")
async def embed(text: str):
    return  { "embeds": model.encode([text]).tolist()[0] }

@app.get("/import-images-embeddings")
async def import_images_embeddings():
    images_folder_path = "./images"

    conn = psycopg2.connect(
        dbname="semantic-search-images-example",
        user="postgres",
        password="postgres_pwd",
        host="localhost",
        port="5432"
    )
    cursor = conn.cursor()

    cursor.execute("""
    DELETE FROM "ImageDocument";
    """)

    # Iterate over all files in the directory
    for root, dirs, files in os.walk(images_folder_path):
        for file in files:
            file_path = os.path.join(root, file)

            print(f"Found file: {file_path}")
            process_image(file_path, cursor, conn)

    cursor.close()
    conn.close()

def process_image(image_path, cursor, conn):
    img = Image.open(image_path)
    img_emb = model.encode(img).tolist()  # Convert to list for PostgreSQL compatibility

    file = open(image_path, 'rb').read()
    file_name = os.path.basename(image_path)

    # Insert the embedding into the table
    cursor.execute("""
    INSERT INTO "ImageDocument" ("Title", "ImageData", "EmbeddingVector")
    VALUES (%s, %s, %s)
    """, (file_name, psycopg2.Binary(file), img_emb,))
    conn.commit()
