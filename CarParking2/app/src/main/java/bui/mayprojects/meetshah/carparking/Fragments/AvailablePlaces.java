package bui.mayprojects.meetshah.carparking.Fragments;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;
import java.util.List;

import bui.mayprojects.meetshah.carparking.R;
import bui.mayprojects.meetshah.carparking.adapters.LocationRecyclerViewAdapter;
import bui.mayprojects.meetshah.carparking.models.Location;
import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * A simple {@link Fragment} subclass.
 */
public class AvailablePlaces extends Fragment {

    @BindView(R.id.rvParking)
    RecyclerView rvParking;
    private List<Location> locations;

    public AvailablePlaces() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment

        View view = inflater.inflate(R.layout.fragment_available_places, container, false);
        ButterKnife.bind(this, view);

        initializeData();

        LinearLayoutManager llm = new LinearLayoutManager(this.getContext());

        rvParking.setHasFixedSize(true);
        rvParking.setLayoutManager(llm);

        LocationRecyclerViewAdapter adapter = new LocationRecyclerViewAdapter(getContext(), locations);
        rvParking.setAdapter(adapter);


        return view;
    }

    private void initializeData(){

        locations = new ArrayList<>();
        locations.add(new Location("277834", "R Mall Mulund", "Average 800 Parkinig Spaces"," "," "));
        locations.add(new Location("121856", "Viviana Mall", "Average 1000 Parking Spaces"," "," "));
        locations.add(new Location("330459", "D-Mart Store", "Average 200 Parking Spaces"," "," "));
        locations.add(new Location("283366", "Mulund Parking For All", "Average 500 Parking Spaces"," "," "));
        locations.add(new Location("313369", "Park By Point", "Average 700 Parking Spaces"," "," "));

    }

}
