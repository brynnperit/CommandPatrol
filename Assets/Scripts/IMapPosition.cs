using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using Priority_Queue;

public interface IMapPosition : IPriorityQueueNode {

	Transform getCenterPathNode();
	Transform getPathNode(int xPosition, int zPosition);
	Direction getDirectionOfAdjacentNonEmptyConnectedMapPosition(IMapPosition adjacentPosition);
	ReadOnlyCollection<IMapPosition> getAdjacents();
	Transform[] getPathThrough(IMapPosition startingPosition, IMapPosition endingPosition, Transform[] toStorePathIn);
	Transform[] getPathThrough(IMapPosition startingPosition, IMapPosition endingPosition);
	int xPosition{ get; set; }
	int zPosition{ get; set; }
	int xSize{ get; set; }
	int zSize{ get; set; }
	bool isEmpty();
	Transform visualRepresentation{ get; set; }
}
