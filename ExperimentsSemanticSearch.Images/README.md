## Pre-requisites
- Python 3.12
- .NET 9

## Getting started

### Python API
- in the `images-embedder` directory of the project, run the following command to create a new python environment:
```
python -m venv python-env
```

- Activate the environment:
```
python-env\Scripts\activate
```

- Install the required packages:
```
pip install -r requirements.txt
```

- Run the API
```
fastapi dev main.py
```

## .NET App
- Run the project
```
dotnet run
```
- When running the project for the first time, selecte `y` as answer to the "Run images embeddings" prompt.
