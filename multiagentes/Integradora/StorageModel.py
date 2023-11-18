from mesa import Model, Agent
from mesa.space import SingleGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector
import math

import numpy as np

# Funcion para calcular la distancia ortogonal entre dos posiciones
def ortogonalDistance(pos_1: tuple[int, int], pos_2: tuple[int, int]):
    return abs(pos_1[0] - pos_2[0]) + abs(pos_1[1] - pos_2[1])

# Constantes para el estado de los agentes 
EXPLORING = 0
COLLECTING = 1
DEPOSITING = 2


# Clase para los agentes
class Agent(Agent):

    def __init__(self, id, model: "StorageModel"):
        super().__init__(id, model)
        self.random.seed(67890)
        self.model = model
        self.boxes_carried = 0
        self.state = EXPLORING
        self.collectionSource = None
        self.steps = 0
        self.sleep = 0
        self.lastDrop = None

    # Funcion para el paso de la simulacion
    def step(self):
        self.steps += 1
        self.scan()

        # Dependiendo del estado del agente, se ejecuta la funcion correspondiente
        if self.state == DEPOSITING:
            self.deposit()
        elif self.state == COLLECTING:
            self.collect()
        else:
            self.explore()

    # Funcion para explorar el almacen
    def explore(self):
        smallestTowers = self.findSmallestTowers()
        # Si encuentra una torre con altura menor a la maxima, se cambia al estado de recoleccion
        if len(smallestTowers) > 0:
            self.state = COLLECTING
            self.collect()
            return

        # Si no, se mueve aleatoriamente
        self.move(None)
    
    # Funcion para recolectar cajas
    def collect(self):
        smallestTowers = self.findSmallestTowers()
        smallestTowers = list(filter(lambda tower: tuple(tower) != self.lastDrop, smallestTowers))
        
        # Si no hay torres con altura menor a la maxima, se cambia al estado de exploracion
        if len(smallestTowers) == 0:
            self.state = EXPLORING
            self.move(None)
            return
        
        # Si una de las posibles torres está al alcance, se recolecta una caja
        neighbourCells = self.model.grid.get_neighborhood(
            self.pos, moore=False, include_center=False)
        
        for cell in neighbourCells:
            if cell in smallestTowers:
                self.boxes_carried += 1
                self.model.real[cell[0]][cell[1]] -= 1
                self.model.known[cell[0]][cell[1]] -= 1
                self.state = DEPOSITING
                return
            
        # Si no, se mueve hacia la torre más cercana
        smallestTowers.sort(key=lambda x: ortogonalDistance(x, self.pos))
        self.move(smallestTowers[0])

    # Funcion para depositar cajas
    def deposit(self):

        # Si no está cargando cajas, se cambia al estado de exploracion 
        if self.boxes_carried == 0:
            self.state = EXPLORING
            self.explore()
            return
    
        biggestTowers = self.findBiggestTowers()

        # Si no tiene donde depositar, se mueve aleatoriamente
        if len(biggestTowers) == 0:
            self.move(None)
            return
        
        # Si una de las posibles torres está al alcance, se deposita una caja
        neighbourCells = self.model.grid.get_neighborhood(
            self.pos, moore=False, include_center=False)
        
        for cell in neighbourCells:
            if cell in biggestTowers:
                self.model.real[cell[0]][cell[1]] += self.boxes_carried
                self.model.known[cell[0]][cell[1]] += self.boxes_carried
                self.boxes_carried = 0
                self.state = EXPLORING
                self.lastDrop = (cell[0], cell[1])
                return
        
        # Si no, se mueve hacia la torre más cercana
        biggestTowers.sort(key=lambda x: ortogonalDistance(x, self.pos))
        self.move(biggestTowers[0])

    # Funcion para encontrar las torres más pequeñas
    def findSmallestTowers(self):
        smallestTower = math.inf
        smallestTowers = []

        for i, row in enumerate(self.model.known):
            for j, cell in enumerate(row):
                if cell <= 0 or cell >= self.model.maxTowerHeight:
                    continue

                if cell < smallestTower:
                    smallestTower = cell
                    smallestTowers = [(i, j)]
                elif cell == smallestTower:
                    smallestTowers.append((i, j))

        return smallestTowers

    # Funcion para encontrar las torres más grandes
    def findBiggestTowers(self):
        biggestTower = 0
        biggestTowers = []

        for i, row in enumerate(self.model.known):
            for j, cell in enumerate(row):
                if cell <= 0 or cell >= self.model.maxTowerHeight:
                    continue

                if cell > biggestTower:
                    biggestTower = cell
                    biggestTowers = [(i, j)]
                elif cell == biggestTower:
                    biggestTowers.append((i, j))

        return biggestTowers

    # Funcion para moverse a una posicion, si es posible, si no a la más cercana
    def move(self, cell: tuple[int, int]):
        if cell == None:
            cell = self.getRandomPossibleMove()
            if cell == None:
                return
        
        possibleMoves = self.getPossibleMoves()
        if len(possibleMoves) == 0:
            return

        if cell in possibleMoves:
            self.model.grid.move_agent(self, cell)
        
        else :
            possibleMoves.sort(key=lambda x: ortogonalDistance(x, cell))
            self.model.grid.move_agent(self, possibleMoves[0])
               
    # Funcion para obtener las posiciones posibles a las que se puede mover
    def getPossibleMoves(self):
        neighbourCells = self.model.grid.get_neighborhood(
        self.pos, moore=False, include_center=False)
        emptyNeighbours = [
            cell for cell in neighbourCells if self.model.is_space_empty(cell)]
        return emptyNeighbours

    # Funcion obtener una posicion aleatoria a la que se puede mover
    def getRandomPossibleMove(self):
        possibleMoves = self.getPossibleMoves()
        if len(possibleMoves) > 0:
            newPosition = self.random.choice(possibleMoves)
            return newPosition
        else:
            return None

    # Funcion para escanear las posiciones alrededor del agente
    def scan(self):
        neighbourCells = self.model.grid.get_neighborhood(
            self.pos, moore=False, include_center=False)
        
        for cell in neighbourCells:
            self.model.known[cell[0]][cell[1]] = self.model.real[cell[0]][cell[1]]



class StorageModel(Model):
    def __init__(self, width, height, agents, boxes, maxInitTowerHeight, maxTowerHeight, render=False):
        self.random.seed(67890)
        self.width = width
        self.height = height
        self.agents = agents
        self.boxes = boxes
        self.maxInitTowerHeight = maxInitTowerHeight
        self.maxTowerHeight = maxTowerHeight
        self.running = True

        self.columns = 0
        self.maxColumns = self.boxes // self.maxTowerHeight

        self.grid = SingleGrid(width, height, False)
        self.real = np.zeros((width, height), dtype=int)
        self.known = np.zeros((width, height), dtype=int)
        # self.known = self.real
        self.schedule = RandomActivation(self)

        reporters = {"Data": self.get_data, "Boxes": self.get_boxes}
        

        if render:
            reporters["Known"] = self.get_known
            reporters["Real"] = self.get_real
            reporters["Agents"] = self.get_agents

        self.datacollector = DataCollector(model_reporters=reporters)

        for i in range(agents):
            a = Agent(i, self)
            (x, y) = self.get_random_coords()
            self.schedule.add(a)
            self.grid.place_agent(a, (x, y))

        boxes_placed = 0
        while boxes_placed < boxes:
            (x, y) = self.get_random_coords()
            ammount = self.random.randint(1, maxInitTowerHeight)
            if boxes_placed + ammount > boxes:
                ammount = boxes - boxes_placed
            boxes_placed += ammount

            self.real[x][y] = ammount        

    def step(self):
        self.datacollector.collect(self)
        self.schedule.step()
        if self.columns >= self.maxColumns:
            self.datacollector.collect(self)
            self.running = False
        mask = self.real >= self.maxTowerHeight
        self.columns = mask.sum()

    def get_known(self):
        return self.known.copy()

    def get_real(self):
        return self.real.copy()

    def get_agents(self):
        grid = np.full((self.width, self.height), False, dtype=bool)
        for (content, (x, y)) in self.grid.coord_iter():
            if content != None:
                grid[x][y] = True
        return grid

    def get_data(self):
        agents = []
        for agent in self.schedule.agents:
            agents.append(agent.pos)
        
        columnsLeft = self.columns

        return {
            "agents": agents,
            "columnsLeft": columnsLeft
        }

    def get_boxes(self):
        sum = 0
        sum += self.real.sum()
        for agent in self.schedule.agents:
            sum += agent.boxes_carried
        return sum

    def get_random_coords(self):
        isEmpty = False
        while not isEmpty:
            x = self.random.randint(0, self.width - 1)
            y = self.random.randint(0, self.width - 1)
            if self.is_space_empty((x, y)) and self.real[x][y] == 0:
                isEmpty = True
        return (x, y)

    def is_space_empty(self, pos):
        return self.grid.is_cell_empty(pos)